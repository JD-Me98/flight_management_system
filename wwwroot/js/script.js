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