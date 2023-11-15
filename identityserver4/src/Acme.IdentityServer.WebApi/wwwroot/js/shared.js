function copyToClipboard(text) {
    navigator.clipboard.writeText(text);

    var copyTextElements = document.getElementsByClassName('secret-key-copy-text-container');
    var copyTextElement = copyTextElements[0];
    copyTextElement.style.display = 'inline';
}

function isCodeValid() {
    var verificationCode = document.getElementById('verification-code');
    var element = document.getElementById('check-mark');

    if (verificationCode && verificationCode.value.length == 6) {
        element.style.display = 'inline';
    }
    else {
        element.style.display = 'none';
    }
}
