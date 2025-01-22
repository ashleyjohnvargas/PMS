document.addEventListener('DOMContentLoaded', function () {
    var urgencyLevels = document.querySelectorAll('td:nth-child(6)'); // Select all 'Urgency Level' cells
    var dateStarted = document.querySelectorAll('td:nth-child(7)'); // Select all 'Date Started' cells
    var dateFinished = document.querySelectorAll('td:nth-child(8)'); // Select all 'Date Finished' cells

    // Apply styles to urgency levels
    urgencyLevels.forEach(function (cell) {
        var urgencyText = cell.textContent.trim();

        if (urgencyText === 'High') {
            cell.classList.add('urgency-high');
        } else if (urgencyText === 'Medium') {
            cell.classList.add('urgency-medium');
        } else if (urgencyText === 'Low') {
            cell.classList.add('urgency-low');
        }
    });

    // Apply red color to "Pending" in Date Started
    dateStarted.forEach(function (cell) {
        var dateText = cell.textContent.trim();

        if (dateText === 'Pending') {
            cell.classList.add('date-started'); // Apply the 'date-started' class to make text red
        }
    });

    dateFinished.forEach(function (cell) {
        var dateText = cell.textContent.trim();

        if (dateText === 'In Progress') {
            cell.classList.add('date-finished-inprogress');
        } else if (dateText === 'Pending') {
            cell.classList.add('date-finished-pending');
        }
    });
});