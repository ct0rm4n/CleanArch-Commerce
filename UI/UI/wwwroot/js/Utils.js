function ShowModal(modalId) {
    $('#' + modalId).modal('show');
}
function CloseModal(modalId) {
    $('#' + modalId).modal('hide');
}
function TestDataTablesAdd(table) {    
    alert(table);
}
$(document).ready(function () {
    console.log("ready!");
    //TestDataTablesAdd("#table")
});

//audio libs
function startRecording() {
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(stream => {
            const recorder = new MediaRecorder(stream, { type: 'audio/webm' }); // Use WEBM for wider browser compatibility

            recorder.ondataavailable = handleData;
            recorder.start();

            // Add stop button listener
            document.getElementById('stop-button').addEventListener('click', () => {
                recorder.stop();
            });
        })
        .catch(error => {
            console.error("Error accessing microphone:", error);
        });
}

function handleData(event) {
    const chunk = event.data;
    DotNet.invokeMethodAsync('Search', 'ReceiveAudioChunk', new Uint8Array(chunk));
}

function stopRecording() {
    // No longer needed as the stop button listener handles this
}

