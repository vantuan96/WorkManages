$(function () {
    $('body').on('click', '.btnSearch', function () {
        MN_KeyCardController.loadData(1, true);
    });
});

var config = {
    changePageSize: false,
    pageIndex: 1
};

var MN_KeyCardController = {
    init() {
        MN_KeyCardController.loadData(1, true);
    },
    loadData(pageIndex, changePageSize) {
        JSHelper.AJAX_LoadData('/MN_KeySecurity/CardPartial', { key: "", keysecurityid: $('#hidKeySecurity').val(), fromdate: $("input[name=fromdate]").val(), todate: $("input[name=todate]").val(), pageindex: pageIndex })
            .success(function (response) {
                var box = $("#boxCard");

                var str = "";

                response.Data.forEach(function (item, index) {
                    str += "<tr>";
                    str += "<td>" + item.CardNo + "</td>";
                    str += "<td>" + item.CardNumber + "</td>";
                    str += "</tr>";
                });

                $("#tblCardList tbody").html('');
                $("#tblCardList tbody").html(str);

                MN_KeyCardController.paging(response.TotalPage, changePageSize);
            });
    },
    pagingEmpty: function () {
        $('#pagination').empty();
        $('#pagination').removeData("twbs-pagination");
        $('#pagination').unbind("page");
    },
    paging: function (totalPage, changePageSize) {
        //debugger;
        //Unbind pagination if it existed or click change pagesize
        if ($('#pagination a').length === 0 || changePageSize === true) {
            $('#pagination').empty();
            $('#pagination').removeData("twbs-pagination");
            $('#pagination').unbind("page");
        }

        $('#pagination').twbsPagination({
            totalPages: totalPage,
            first: "<<",
            next: ">",
            last: ">>",
            prev: "<",
            visiblePages: 5,
            paginationClass: "pagination pagination-sm",
            initiateStartPageClick: false,
            onPageClick: function (event, page) {
                //config.pageIndex = page;

                MN_KeyCardController.loadData(page, false);
            }
        });
    },
}