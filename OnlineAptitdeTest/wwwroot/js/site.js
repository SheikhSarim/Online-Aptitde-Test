const testDuration = 3;

const endTime = new Date();
endTime.setMinutes(endTime.getMinutes() + testDuration);

const timerElement = document.getElementById('timer');

const updateTimer = () => {
    const currentTime = new Date();
    const remainingTime = new Date(endTime - currentTime);

    const minutes = remainingTime.getUTCMinutes();
    const seconds = remainingTime.getUTCSeconds();
    timerElement.textContent = `${minutes}:${seconds < 10 ? '0' : ''}${seconds}`;

    if (currentTime >= endTime) {
        clearInterval(timerInterval);
    }
};

const timerInterval = setInterval(updateTimer, 1000);