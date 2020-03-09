var myVar;

function countDown() {
    myVar = setInterval(getInfoCount, 5000);
}

function getInfoCount() {
    alert("");
    var countDownDate = new Date("Dec 7, 2019 12:00:00").getTime();
    // Get today's date and time
    var now = new Date().getTime();

    // Find the distance between now and the count down date
    var distance = countDownDate - now;

    // Time calculations for days, hours, minutes and seconds
    var days = Math.floor(distance / (1000 * 60 * 60 * 24));
    var hours = Math.floor((distance % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
    var minutes = Math.floor((distance % (1000 * 60 * 60)) / (1000 * 60));
    var seconds = Math.floor((distance % (1000 * 60)) / 1000);

    // Output the result in an element with id="demo"
    document.getElementById("figure_days").innerHTML = days
    document.getElementById("figure_hours").innerHTML = hours
    document.getElementById("figure_mins").innerHTML = minutes

    // If the count down is over, write some text 
    if (distance < 0) {
        clearInterval(myVar);
    }
}
