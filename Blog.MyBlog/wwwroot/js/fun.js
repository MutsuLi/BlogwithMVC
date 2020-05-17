//节日
if (true) {
    let txt = null;
    switch ((new Date(new Date().valueOf() + 8 * 3600000)).toISOString().substring(5, 10)) {
        case "10-01":
        case "10-02":
        case "10-03":
        case "10-04":
        case "10-05":
        case "10-06":
            txt = "伟大的中华人民共和国万岁!";
            break;
        case "01-01":
            txt = "元旦快乐哟!";
            break;
    }

    if (txt && location.pathname.indexOf("/draw/mind") == -1) {
        var dh = document.createElement('div');
        dh.innerHTML = '<div class="d-none d-md-block text-center h4 py-2 text-warning bg-danger">' + txt.split('').join(' ') + '</div>';
        document.body.insertBefore(dh, document.body.firstChild);
    }
}