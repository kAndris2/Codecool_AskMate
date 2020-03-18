// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function toggleRate(element) {
    if (element.id == "rateUp") {
        console.log("toggled up! :)");
        element.style.opacity = 1;
    }
    else {
        console.log("toggled down");
        element.style.opacity = 1;
    }
    console.log(element);
}