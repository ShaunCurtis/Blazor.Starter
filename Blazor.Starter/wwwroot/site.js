
window.SetBodyCss = function (elementId, classname) {
    var link = document.getElementById(elementId);
    if (link !== undefined) {
        link.className = classname;
    }
    return true;
}

window.SetBaseHref = function (elementId, hRef) {
    var link = document.getElementById(elementId);
    if (link !== undefined) {
        link.href = hRef;
    }
    return true;
}

window.SetBackPage = function (url) {
    history.pushState(null, "", url);
    return true;
}

window.SetElementFocus = function (element) {
    element.focus();
    return element === true;
}

window.SetFocusByElementId = function (elementId) {
    var element = document.getElementById(elementId);
    element.focus();
    return element === true;
}

window.GetElementRef = function (elementId) {
    var element = document.getElementById(elementId);
    return element;
}
