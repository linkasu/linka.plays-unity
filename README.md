# linka.plays-unity

`TTS.SpeakAndSaveAudio` sends `POST` JSON to `https://tts.linka.su/tts` with the default `jane` voice and saves the returned MP3 in `Assets/Audio`.

Set `LINKA_TTS_URL` to use an operator endpoint such as `/v1/tts/anonymous`. Optionally set `LINKA_TTS_INSTALLATION_TOKEN`; it is sent only as `X-TTS-Installation-Token` with a per-request `Idempotency-Key`. Do not commit either value.
