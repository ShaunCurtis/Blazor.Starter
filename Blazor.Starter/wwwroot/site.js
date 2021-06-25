
window.SetBodyCss = function (elementId, classname) {
    var link = document.getElementById(elementId);
    if (link !== undefined) {
        link.className = classname;
    }
    return true;
}
