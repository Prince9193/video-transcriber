"""Tiny stand-in for server.py so the .NET gateway can be tested without FFmpeg/ImageMagick.
Run:  python tools/mock_flask.py   (listens on :5000, same routes as the real Flask app)"""
import os, time
from flask import Flask, request, jsonify, send_file

app = Flask(__name__)
os.makedirs("mock_uploads", exist_ok=True)

@app.get("/api/languages")
def languages():
    return jsonify({"hi": "Hindi", "fr": "French", "fail": "Always fails (test)"})

@app.post("/api/translate")
def translate():
    f = request.files.get("video"); lang = request.form.get("language")
    if not f or not lang:
        return jsonify({"status": "error", "message": "Missing required parameters"}), 400
    time.sleep(2)  # pretend the pipeline is working
    if lang == "fail":
        return jsonify({"status": "error", "message": "Simulated pipeline failure"}), 500
    out = f"translated_{int(time.time())}.mp4"
    f.save(os.path.join("mock_uploads", out))
    return jsonify({"status": "success", "message": "ok", "output_file": out})

@app.get("/api/download/<name>")
def download(name):
    return send_file(os.path.abspath(os.path.join("mock_uploads", name)), as_attachment=True)

if __name__ == "__main__":
    app.run(port=5000)
