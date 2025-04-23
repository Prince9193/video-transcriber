// Get DOM elements
const translationForm = document.getElementById('translationForm');
const videoInput = document.getElementById('videoInput');
const languageSelect = document.getElementById('languageSelect');
const translateButton = document.getElementById('translateButton');
const statusDiv = document.getElementById('status');
const downloadSection = document.getElementById('downloadSection');
const downloadButton = document.getElementById('downloadButton');

let currentOutputFile = null;

// Update translate button state
function updateTranslateButton() {
    const hasVideo = videoInput.files.length > 0;
    const hasLanguage = languageSelect.value !== '';
    
    translateButton.disabled = !(hasVideo && hasLanguage);
}

// Show status message
function showStatus(message, type = 'info') {
    statusDiv.textContent = message;
    statusDiv.className = 'status ' + type;
}

// Show download section
function showDownloadSection(filename) {
    currentOutputFile = filename;
    downloadSection.style.display = 'block';
}

// Hide download section
function hideDownloadSection() {
    downloadSection.style.display = 'none';
    currentOutputFile = null;
}

// Handle file input change
videoInput.addEventListener('change', () => {
    const file = videoInput.files[0];
    if (file) {
        // Check if file is a video
        if (!file.type.startsWith('video/')) {
            showStatus('Please select a video file', 'error');
            videoInput.value = '';
            return;
        }
    }
    updateTranslateButton();
    hideDownloadSection();
});

// Handle language selection change
languageSelect.addEventListener('change', () => {
    updateTranslateButton();
    hideDownloadSection();
});

// Handle form submission
translationForm.addEventListener('submit', async (e) => {
    e.preventDefault();
    
    const formData = new FormData(translationForm);
    
    try {
        translateButton.disabled = true;
        showStatus('Processing... Please wait', 'info');
        hideDownloadSection();
        
        const response = await fetch('/api/translate', {
            method: 'POST',
            body: formData
        });
        
        const result = await response.json();
        
        if (result.status === 'success') {
            showStatus(result.message, 'success');
            showDownloadSection(result.output_file);
        } else {
            showStatus(result.message, 'error');
        }
    } catch (error) {
        showStatus('Error: ' + error, 'error');
    } finally {
        translateButton.disabled = false;
    }
});

// Handle download button click
downloadButton.addEventListener('click', async () => {
    if (!currentOutputFile) return;
    
    try {
        window.location.href = `/api/download/${currentOutputFile}`;
    } catch (error) {
        showStatus('Error downloading file: ' + error, 'error');
    }
});

// Initialize the application
updateTranslateButton(); 