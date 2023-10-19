$(document).ready(function () {

    $("#Username,#Password").keyup(function (e) {
        evaluateCapLock(e.originalEvent);
    });
    $("#revealPassword").click(togglePasswordVisibility);

    function evaluateCapLock(e) {
        var warning = $("#capsWarning");
        // If "caps lock" is pressed, display the warning text
        if (!!e.getModifierState && e.getModifierState("CapsLock")) {
            warning.attr("class", "warning");
        } else {
            warning.attr("class", "warning hidden");
        }
    }

    function togglePasswordVisibility() {
        var password = $("#Password");
        var revealBtn = $("#revealPassword");
        if (password.attr("type") == "password") {
            password.attr("type", "text");
            revealBtn.text("Hide");
        } else {
            password.attr("type", "password");
            revealBtn.text("Show");
        }
    }
});

