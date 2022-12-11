import base64
import json
import queue
import threading

from pyaudio import PyAudio, paContinue, paFloat32
from cochl_sense_api import Configuration, ApiClient 
from cochl_sense_api.api import audio_session_api
from cochl_sense_api.model.audio_chunk import AudioChunk
from cochl_sense_api.model.audio_type import AudioType
from cochl_sense_api.model.create_session import CreateSession
import UdpComms as U

configuration = Configuration()
configuration.api_key['API_Key'] = 'please_insert_your_api_key_here'

# Create UDP socket to use for sending (and receiving)
sock = U.UdpComms(udpIP="127.0.0.1", portTX=8000, portRX=8001, enableRX=True, suppressWarnings=True)

client = ApiClient(configuration)
api = audio_session_api.AudioSessionApi(client)
created = api.create_session(CreateSession(
    content_type="audio/x-raw; rate=22050; format=f32",
    type=AudioType("stream"),
))
id = created.session_id

class PyAudioSense:
    def __init__(self):
        self.rate = 22050
        chunk = int(self.rate / 2)
        self.buff = queue.Queue()
        self.audio_interface = PyAudio()
        self.audio_stream = self.audio_interface.open(
             format=paFloat32,
             channels=1, rate=self.rate,
             input=True, frames_per_buffer=chunk,
             stream_callback=self._fill_buffer
        )

    def _fill_buffer(self, in_data, frame_count, time_info, status_flags):
        self.buff.put(in_data)
        return None, paContinue

    def generator(self):
        while True:
            chunk = self.buff.get()
            if chunk is None:
                return
            yield chunk

    def upload(self):
        seq = 0
        for chunk in self.generator():
            encoded = base64.b64encode(chunk).decode("utf-8")
            uploaded = api.upload_chunk(session_id=id, chunk_sequence=seq, audio_chunk=AudioChunk(encoded))
            seq = uploaded.chunk_sequence

#upload audio chunk from microphone in other thread
microphone = PyAudioSense()
thread = threading.Thread(target=microphone.upload)
thread.start()

#get results
next_token=""
while True:
    resp = api.read_status(session_id=id, next_token=next_token)
    for result in resp.inference.results:
        sound_type = result["tags"][0]["name"]
        print(sound_type)
        sock.SendData(sound_type)

    if "next_token" in resp.inference.page:
        next_token = resp.inference.page.next_token
    else:
        break