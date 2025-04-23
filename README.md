# Video Transcriber and Translator

A Python-based web application that transcribes, translates, and adds subtitles to videos. This project can process both short and long videos by automatically splitting them into manageable chunks.

## Features

- Extract audio from video files
- Transcribe audio using Google Speech Recognition
- Translate transcribed text to 100+ languages
- Generate subtitles with customizable styling
- Create translated audio using Google Text-to-Speech
- Process large videos by splitting them into chunks
- Combine translated audio and subtitles with original video
- Modern web interface with real-time progress updates
- Support for multiple video formats (MP4, AVI, MOV, WMV)
- Automatic cleanup of temporary files

## Prerequisites

- Python 3.7 or higher
- ImageMagick (for text overlay)
- FFmpeg (for video processing)
- Modern web browser (Chrome recommended)

## Installation

1. Clone the repository:
```bash
git clone https://github.com/yourusername/video-transcriber.git
cd video-transcriber
```

2. Install required Python packages:
```bash
pip install -r requirements.txt
```

3. Install ImageMagick:
   - Download and install ImageMagick from [ImageMagick's official website](https://imagemagick.org/script/download.php)
   - Make sure to check "Install legacy utilities" during installation
   - Add ImageMagick to your system's PATH

4. Install FFmpeg:
   - Download FFmpeg from [FFmpeg's official website](https://ffmpeg.org/download.html)
   - Add FFmpeg to your system's PATH

## Usage

1. Start the application:
```bash
python server.py
```

2. Open your web browser and navigate to:
```
http://localhost:5000
```

3. Upload a video file and select the target language
4. Wait for the translation process to complete
5. Download the translated video with subtitles

## Supported Languages

The application supports translation to the following languages:
- Afrikaans (af)
- Albanian (sq)
- Amharic (am)
- Arabic (ar)
- Armenian (hy)
- Assamese (as)
- Aymara (ay)
- Azerbaijani (az)
- Bambara (bm)
- Basque (eu)
- Belarusian (be)
- Bengali (bn)
- Bhojpuri (bho)
- Bosnian (bs)
- Bulgarian (bg)
- Catalan (ca)
- Cebuano (ceb)
- Chichewa (ny)
- Chinese (Simplified) (zh-CN)
- Chinese (Traditional) (zh-TW)
- Corsican (co)
- Croatian (hr)
- Czech (cs)
- Danish (da)
- Dhivehi (dv)
- Dogri (doi)
- Dutch (nl)
- English (en)
- Esperanto (eo)
- Estonian (et)
- Ewe (ee)
- Filipino (tl)
- Finnish (fi)
- French (fr)
- Frisian (fy)
- Galician (gl)
- Georgian (ka)
- German (de)
- Greek (el)
- Guarani (gn)
- Gujarati (gu)
- Haitian Creole (ht)
- Hausa (ha)
- Hawaiian (haw)
- Hebrew (iw)
- Hindi (hi)
- Hmong (hmn)
- Hungarian (hu)
- Icelandic (is)
- Igbo (ig)
- Ilocano (ilo)
- Indonesian (id)
- Irish (ga)
- Italian (it)
- Japanese (ja)
- Javanese (jw)
- Kannada (kn)
- Kazakh (kk)
- Khmer (km)
- Kinyarwanda (rw)
- Konkani (gom)
- Korean (ko)
- Krio (kri)
- Kurdish (Kurmanji) (ku)
- Kurdish (Sorani) (ckb)
- Kyrgyz (ky)
- Lao (lo)
- Latin (la)
- Latvian (lv)
- Lingala (ln)
- Lithuanian (lt)
- Luganda (lg)
- Luxembourgish (lb)
- Macedonian (mk)
- Maithili (mai)
- Malagasy (mg)
- Malay (ms)
- Malayalam (ml)
- Maltese (mt)
- Maori (mi)
- Marathi (mr)
- Meiteilon (Manipuri) (mni-Mtei)
- Mizo (lus)
- Mongolian (mn)
- Myanmar (my)
- Nepali (ne)
- Norwegian (no)
- Odia (Oriya) (or)
- Oromo (om)
- Pashto (ps)
- Persian (fa)
- Polish (pl)
- Portuguese (pt)
- Punjabi (pa)
- Quechua (qu)
- Romanian (ro)
- Russian (ru)
- Samoan (sm)
- Sanskrit (sa)
- Scots Gaelic (gd)
- Sepedi (nso)
- Serbian (sr)
- Sesotho (st)
- Shona (sn)
- Sindhi (sd)
- Sinhala (si)
- Slovak (sk)
- Slovenian (sl)
- Somali (so)
- Spanish (es)
- Sundanese (su)
- Swahili (sw)
- Swedish (sv)
- Tajik (tg)
- Tamil (ta)
- Tatar (tt)
- Telugu (te)
- Thai (th)
- Tigrinya (ti)
- Tsonga (ts)
- Turkish (tr)
- Turkmen (tk)
- Twi (ak)
- Ukrainian (uk)
- Urdu (ur)
- Uyghur (ug)
- Uzbek (uz)
- Vietnamese (vi)
- Welsh (cy)
- Xhosa (xh)
- Yiddish (yi)
- Yoruba (yo)
- Zulu (zu)


## Technical Details

- Maximum video size: It can translate only 20 seconds video
- Supported video formats: MP4, AVI, MOV, WMV
- Audio processing: 16kHz sample rate
- Video codec: H.264
- Subtitle format: Embedded in video

## Troubleshooting

1. If you encounter "ImageMagick not found" error:
   - Verify ImageMagick installation
   - Check system PATH
   - Restart the application

2. If video processing fails:
   - Check FFmpeg installation
   - Ensure sufficient disk space
   - Try a smaller video file

3. If translation fails:
   - Check internet connection
   - Verify language code is supported
   - Try a different video format

## Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Acknowledgments

- Google Speech Recognition API
- Google Translate API
- Google Text-to-Speech API
- MoviePy library
- FFmpeg
- ImageMagick
- Eel framework 