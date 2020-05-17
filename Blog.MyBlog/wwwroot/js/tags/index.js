var gd1 = z.Grid();
gd1.url = "/tags/QueryTagList";
gd1.autosizePid = "#PGrid1";
gd1.pageSize = 100;
gd1.multiSort = true;
gd1.fitColumns = true;
gd1.sortName = "TagId";
gd1.sortOrder = "desc";
gd1.columns = [[
    { title: "标签", field: "TagName", width: 480, sortable: true },
    { title: "修改时间", field: "updateTime", width: 150, sortable: true, align: "center" },
    { title: "标签状态", field: "TagStatus", width: 150, sortable: true, align: "center" }
]];
gd1.onBeforeLoad = function (row, param) {
    param.pe1 = $('#txtSearch').val().trim();
}
gd1.load();

//搜索
$('#txtSearch').keydown(function (e) {
    console.debug(e);
    e = e || window.event;
    if (e.keyCode == 13) {
        $('#btnSearch')[0].click();
    }
})
$('#btnSearch').click(function () {
    gd1.pageNumber = 1;
    gd1.load();
});


var noteid;

//保存
$('#btnAdd').click(function () {
    var tagName = $('#txtSearch').val().trim();
    var errmsg = [];
    if (tagName == "") {
        errmsg.push("请输入 标签");
    }

    if (errmsg.length > 0) {
        jz.alert(errmsg.join('<br/>'));
        return false;
    }

    $('#btnAdd')[0].disabled = true;

    $.ajax({
        url: "/Tags/AddTag",
        type: "post",
        data: {
            tagName,
        },
        dataType: 'json',
        success: function (data) {
            if (data.code == 200) {
                if (tagName == 0) {
                    tagName = data.TagName;
                }
                $('#txtSearch')[0].value = "";
                gd1.pageNumber = 1;
                gd1.pe1 = "";
                gd1.load();
            }
            jz.msg(data.msg)
        },
        error: function (ex) {
            if (ex.status == 401) {
                jz.msg("请登录");
            } else {
                jz.msg("网络错误");
            }
        },
        complete: function () {
            $('#btnAdd')[0].disabled = false;
        }
    });

});
//删除
$('#btnDel').click(function () {
    var rowData = gd1.func('getSelected');
    if (rowData) {
        jz.confirm({
            content: "确定删除？",
            ok: function () {
                $.ajax({
                    url: "/Tags/DelTag?TagName=" + rowData.TagName,
                    dataType: 'json',
                    success: function (data) {
                        if (data.code == 0) {
                            $('#txtSearch')[0].value = "";
                            gd1.pageNumber = 1;
                            gd1.pe1 = "";
                            gd1.load();
                        } else {
                            jz.msg("操作失败");
                        }
                    },
                    error: function () {
                        jz.msg("网络错误");
                    }
                })
            }
        });
    } else {
        jz.msg("请选择一行再操作");
    }

});

$(window).on('resize', function () {
    mdautoheight();
});

$('#ModalNote').on('shown.bs.modal', function () {
    mdautoheight();
    if (noteid == 0) {
        $('#NoteTitle')[0].focus();
    }
})

function mdautoheight() {
    var vh = $(window).height() - 130;
    nmd.height(Math.max(100, vh));
}