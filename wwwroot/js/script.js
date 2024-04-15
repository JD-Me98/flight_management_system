function dismissError() {
    document.getElementById('Message').style.display = 'none';
}


//Slideshow scripts
var slideIndex = 1;
    var slideInterval = setInterval(function() { plusSlides(1) }, 4000); // Switch slides every 4 seconds

    function plusSlides(n) {
        clearInterval(slideInterval); // Clear previous interval
        slideInterval = setInterval(function() { plusSlides(1) }, 4000); // Reset interval
        showSlides(slideIndex += n);
    }

    function currentSlide(n) {
        clearInterval(slideInterval); // Clear previous interval
        slideInterval = setInterval(function() { plusSlides(1) }, 4000); // Reset interval
        showSlides(slideIndex = n);
    }

    function showSlides(n) {
        var i;
        var slides = document.getElementsByClassName("mySlides");
        var dots = document.getElementsByClassName("dot");
        if (n > slides.length) { slideIndex = 1 }
        if (n < 1) { slideIndex = slides.length }
        for (i = 0; i < slides.length; i++) {
            slides[i].style.display = "none";
        }
        for (i = 0; i < dots.length; i++) {
            dots[i].className = dots[i].className.replace(" active", "");
        }
        slides[slideIndex - 1].style.display = "block";
        dots[slideIndex - 1].className += " active";
}

//filterTable
document.getElementById("filterForm").addEventListener("submit", function (event) {
    event.preventDefault(); // Prevent form submission

    var filterValue = document.getElementById("filterInput").value.toLowerCase();
    var selectedColumnIndex = document.querySelector('input[name="columnRadio"]:checked').value;
    var tableRows = document.getElementById("dataTable").getElementsByTagName("tr");

    for (var i = 1; i < tableRows.length; i++) { // Start from index 1 to skip table header row
        var row = tableRows[i];
        var cell = row.getElementsByTagName("td")[selectedColumnIndex]; // Use selected column index

        if (cell) {
            var cellValue = cell.textContent.toLowerCase();
            if (cellValue.includes(filterValue)) {
                row.style.display = ""; // Show row if it matches filter criteria
            } else {
                row.style.display = "none"; // Hide row if it doesn't match filter criteria
            }
        }
    }
});


//pagination

const totalRows = document.getElementById("dataTable").rows.length - 1; // Subtract 1 for header row
const rowsPerPage = 10; // Adjust as needed
let currentPage = 1;

function showRows(start, end) {
    const tableRows = document.getElementById("tableBody").rows;

    for (let i = 0; i < tableRows.length; i++) {
        if (i >= start && i < end) {
            tableRows[i].style.display = ""; // Show rows within the current page range
        } else {
            tableRows[i].style.display = "none"; // Hide rows outside the current page range
        }
    }
}

function updatePaginationInfo() {
    const pageInfoSpan = document.getElementById("pageInfo");
    const totalPages = Math.ceil(totalRows / rowsPerPage);
    pageInfoSpan.textContent = `Page ${currentPage} of ${totalPages}`;

    const pageNumbersContainer = document.getElementById("pageNumbers");
    pageNumbersContainer.innerHTML = ''; // Clear existing page numbers

    for (let i = 1; i <= totalPages; i++) {
        const pageNumberButton = document.createElement("button");
        pageNumberButton.textContent = i;
        pageNumberButton.addEventListener("click", function () {
            currentPage = i;
            const start = (currentPage - 1) * rowsPerPage;
            const end = currentPage * rowsPerPage;
            showRows(start, end);
            updatePaginationInfo();
        });
        pageNumbersContainer.appendChild(pageNumberButton);
    }
}

document.getElementById("prevPageBtn").addEventListener("click", function () {
    if (currentPage > 1) {
        currentPage--;
        const start = (currentPage - 1) * rowsPerPage;
        const end = currentPage * rowsPerPage;
        showRows(start, end);
        updatePaginationInfo();
    }
});

document.getElementById("nextPageBtn").addEventListener("click", function () {
    const totalPages = Math.ceil(totalRows / rowsPerPage);
    if (currentPage < totalPages) {
        currentPage++;
        const start = (currentPage - 1) * rowsPerPage;
        const end = currentPage * rowsPerPage;
        showRows(start, end);
        updatePaginationInfo();
    }
});

// Show initial page
showRows(0, rowsPerPage);
updatePaginationInfo();