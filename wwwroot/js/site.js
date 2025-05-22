function myFunction() {
    var x = document.getElementById("myInput");
    if (x.type === "password") {
        x.type = "text";
    } else {
        x.type = "password";
    }
}

function myFunction2() {
    var x = document.getElementById("myInput2");
    if (x.type === "password") {
        x.type = "text";
    }
    else {
        x.type = "password";
    }
}

function contentVisibility() {
    var x = document.getElementById("content");
    if (x.style.display === "none") {
        x.style.display = "block";
    } else {
        x.style.display = "none";
    }
}

function showForm() {
    document.getElementById('answer').style.display = "block";
}

function hideForm() {
    document.getElementById("check").style.display = "none";
}

    let mybutton = document.getElementById("topBtn");

    // При скролване на страницата 20px надолу се визуализира бутона за връщане нагоре
    window.onscroll = function() {scrollFunction()};

    function scrollFunction() {
  if (document.body.scrollTop > 20 || document.documentElement.scrollTop > 20) {
        mybutton.style.display = "block";
  }
  else {
        mybutton.style.display = "none";
  }
}

    // Връща се в началото на страницата при натискане на бутона 
    function topFunction() {
        document.body.scrollTop = 0;
    document.documentElement.scrollTop = 0;
}