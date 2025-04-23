import os
os.environ["IMAGEMAGICK_BINARY"] = r"C:\Program Files\ImageMagick-7.1.1-Q16-HDRI\magick.exe"


import time
import moviepy.editor as mp
import speech_recognition as sr
from deep_translator import GoogleTranslator
from gtts import gTTS
from pydub import AudioSegment
import sys
import subprocess
from moviepy.editor import VideoFileClip, AudioFileClip, CompositeVideoClip
from googletrans import Translator
import wave
import contextlib
import math



# Extract audio from video
def extract_audio(video_path, audio_path):
    video = mp.VideoFileClip(video_path)
    audio = video.audio
    audio.write_audiofile(audio_path)

# Convert audio to ensure compatibility
def convert_audio(input_audio_path, output_audio_path):
    audio = AudioSegment.from_file(input_audio_path)
    audio = audio.set_frame_rate(16000)  # Set to 16kHz
    audio.export(output_audio_path, format="wav")

# Transcribe audio
def transcribe_audio(audio_path):
    recognizer = sr.Recognizer()
    audio_file = sr.AudioFile(audio_path)
    with audio_file as source:
        audio_data = recognizer.record(source)
    try:
        transcript = recognizer.recognize_google(audio_data)
        return transcript
    except sr.RequestError as e:
        print(f"Could not request results from Google Speech Recognition service; {e}")
    except sr.UnknownValueError:
        print("Google Speech Recognition could not understand audio")
    return None

# Translate text
def translate_text(text, dest_language='en'):
    translator = GoogleTranslator(source='auto', target=dest_language)
    translation = translator.translate(text)
    return translation

# Split transcript into timed chunks
def split_transcript(transcript, chunk_length=10):
    words = transcript.split()
    chunks = []
    current_chunk = []
    current_length = 0
    for word in words:
        current_chunk.append(word)
        current_length += len(word) + 1
        if current_length >= chunk_length:
            chunks.append(" ".join(current_chunk))
            current_chunk = []
            current_length = 0
    if current_chunk:
        chunks.append(" ".join(current_chunk))
    return chunks

# Create subtitles clip
def create_subtitles_clip(video_path, transcript, output_path):
    video = mp.VideoFileClip(video_path)
    transcript_chunks = split_transcript(transcript)
    subtitles_clips = []
    chunk_duration = video.duration / len(transcript_chunks)
    for i, chunk in enumerate(transcript_chunks):
        subtitle = mp.TextClip(chunk, fontsize=24, color='white', bg_color='black', size=(video.size[0], 50))
        subtitle = subtitle.set_duration(chunk_duration).set_position(('center', 'bottom')).set_start(i * chunk_duration)
        subtitles_clips.append(subtitle)
    final_clip = mp.CompositeVideoClip([video, *subtitles_clips])
    final_clip.write_videofile(output_path, codec='libx264', audio=False)

# Create translated audio
def create_translated_audio(transcript, output_audio_path, lang='hi'):
    try:
        # Split text into sentences for better TTS quality
        sentences = [s.strip() for s in transcript.split('.') if s.strip()]
        
        # Create temporary directory for audio chunks
        temp_dir = 'temp_audio_chunks'
        os.makedirs(temp_dir, exist_ok=True)
        
        # Generate audio for each sentence
        audio_chunks = []
        for i, sentence in enumerate(sentences):
            if not sentence:
                continue
                
            chunk_path = os.path.join(temp_dir, f'chunk_{i}.mp3')
            try:
                tts = gTTS(text=sentence, lang=lang, slow=False)
                tts.save(chunk_path)
                audio_chunks.append(chunk_path)
            except Exception as e:
                print(f"Error processing sentence {i}: {e}")
                continue
        
        if not audio_chunks:
            raise Exception("No audio chunks were generated successfully")
        
        # Combine audio chunks
        combined_audio = AudioSegment.empty()
        for chunk_path in audio_chunks:
            try:
                audio = AudioSegment.from_mp3(chunk_path)
                combined_audio += audio
            except Exception as e:
                print(f"Error combining audio chunk {chunk_path}: {e}")
                continue
        
        # Export final audio
        combined_audio.export(output_audio_path, format="mp3")
        
        # Clean up temporary files
        for chunk in audio_chunks:
            try:
                os.remove(chunk)
            except:
                pass
        try:
            os.rmdir(temp_dir)
        except:
            pass
            
    except Exception as e:
        print(f"Error in create_translated_audio: {e}")
        raise

# Combine video with translated audio and subtitles
def combine_video_audio(video_path, audio_path, subtitles_path, output_path):
    video = mp.VideoFileClip(video_path)
    audio = mp.AudioFileClip(audio_path)
    subtitles = mp.VideoFileClip(subtitles_path).set_duration(video.duration)
    video_with_audio = video.set_audio(audio)
    final_clip = mp.CompositeVideoClip([video_with_audio, subtitles])
    final_clip.write_videofile(output_path, codec='libx264')

# Process a chunk of the video
def process_video_chunk(video_chunk_path, output_chunk_path, dest_language):
    extract_audio(video_chunk_path, 'temp_audio.wav')
    convert_audio('temp_audio.wav', 'converted_audio.wav')
    transcript = transcribe_audio('converted_audio.wav')
    if not transcript:
        print(f"Transcription failed for chunk: {video_chunk_path}")
        return
    translated_text = translate_text(transcript, dest_language)
    create_subtitles_clip(video_chunk_path, translated_text, 'temp_subtitles.mp4')
    create_translated_audio(translated_text, 'translated_audio.mp3', lang=dest_language)
    combine_video_audio(video_chunk_path, 'translated_audio.mp3', 'temp_subtitles.mp4', output_chunk_path)

# Process large video by splitting it into chunks
def process_large_video(input_video_path, output_video_path, dest_language='hi'):
    video = mp.VideoFileClip(input_video_path)
    chunk_duration = 20  # Process 1-minute chunks
    chunks = [video.subclip(start, min(start + chunk_duration, video.duration)) for start in range(0, int(video.duration), chunk_duration)]
    
    temp_chunk_paths = []
    for i, chunk in enumerate(chunks):
        chunk_path = f'temp_chunk_{i}.mp4'
        output_chunk_path = f'temp_output_chunk_{i}.mp4'
        chunk.write_videofile(chunk_path, codec='libx264')
        process_video_chunk(chunk_path, output_chunk_path, dest_language)
        temp_chunk_paths.append(output_chunk_path)
    
    # Merge all chunks
    final_clips = [mp.VideoFileClip(chunk_path) for chunk_path in temp_chunk_paths]
    final_video = mp.concatenate_videoclips(final_clips)
    final_video.write_videofile(output_video_path, codec='libx264')

    # Clean up temporary files
    for file in temp_chunk_paths:
        if os.path.exists(file):
            try:
                os.remove(file)
                print(f"Deleted {file}")
            except PermissionError:
                print(f"Could not delete {file}. It may still be in use.")
            except FileNotFoundError:
                print(f"File {file} not found. Skipping.")

# Main function to run the video translation
def translate_video(input_video_path, output_video_path, dest_language='en'):
    if os.path.getsize(input_video_path) > 100 * 1024 * 1024:  # Example threshold of 100MB for large video
        process_large_video(input_video_path, output_video_path, dest_language)
    else:
        audio_path = 'temp_audio.wav'
        converted_audio_path = 'converted_audio.wav'
        translated_audio_path = 'translated_audio.mp3'
        temp_subtitle_path = 'temp_subtitles.mp4'

        # Extract and transcribe audio
        extract_audio(input_video_path, audio_path)
        convert_audio(audio_path, converted_audio_path)
        transcript = transcribe_audio(converted_audio_path)
        if not transcript:
            print("Transcription failed.")
            return

        # Translate and create subtitles
        translated_text = translate_text(transcript, dest_language)
        create_subtitles_clip(input_video_path, translated_text, temp_subtitle_path)

        # Create translated audio
        create_translated_audio(translated_text, translated_audio_path, lang=dest_language)

        # Combine video with translated audio and subtitles
        try:
            combine_video_audio(input_video_path, translated_audio_path, temp_subtitle_path, output_video_path)
        except Exception as e:
            print(f"Error writing final video file: {e}")

        # Wait for a short time to ensure files are not in use
        time.sleep(15)

        # Clean up temporary files
        temp_files = [audio_path, converted_audio_path, translated_audio_path, temp_subtitle_path]
        for file in temp_files:
            if os.path.exists(file):
                try:
                    os.remove(file)
                    print(f"Deleted {file}")
                except PermissionError:
                    print(f"Could not delete {file}. It may still be in use.")
                except FileNotFoundError:
                    print(f"File {file} not found. Skipping.")