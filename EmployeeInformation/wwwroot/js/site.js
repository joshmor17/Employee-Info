function DisableBackButtonAllBrowsers() {
    window.history.forward()
};
DisableBackButtonAllBrowsers();
window.onload = DisableBackButtonAllBrowsers;
window.onpageshow = function (evts) { if (evts.persisted) DisableBackButtonAllBrowsers(); };
window.onunload = function () { void (0) };
