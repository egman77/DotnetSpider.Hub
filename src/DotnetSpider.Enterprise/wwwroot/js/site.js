// Write your Javascript code.
var dsApp = {};
dsApp.post = function (url, data, success, error) {
    $.post(url, data, function (result, status, request) {
        if (status === "success") {
            if (result && result.success) {
                if (success) {
                    success(result);
                }
            } else {
                if (error) {
                    error(result);
                } else {
                    if (swal) {
                        if (result.message) {
                            swal("Oops...", result.message, "error");
                        }
                    }
                }
            }
        } else {
            if (error) {
                error(result);
            }
            else {
                if (swal) {
                    if (result.message) {
                        swal("Oops...", result.message, "error");
                    }
                }
            }
        }
    }).error(function (data, status, result) {
        if (error) {
            error(result);
        } else {
            if (swal) {
                swal("Oops...", result, 'error');
            }
        }
    });
}

dsApp.get = function (url, success, error) {
    $.get(url, function (result, status, request) {
        if (result && result.success) {
            if (success) {
                success(result);
            }
        } else {
            if (error) {
                error(result);
            }
            else {
                if (swal) {
                    if (result.message) {
                        swal(result.message);
                    }
                }
            }
        }
    }).error(function (data, status, result) {
        if (error) {
            error(result);
        } else {
            if (swal) {
                swal("Oops...", result.message, "Internal error.");
            }
        }
    });
}

dsApp.ui = {};

dsApp.ui.setBusy = function () {
    $("#loadMask").css("display", "");
}
dsApp.ui.clearBusy = function () {
    $("#loadMask").css("display", "none");
}

dsApp.pagers = {};
dsApp.ui.initPagination = function (query, option, click) {
    var total = option.total || 1;
    var size = option.size || 10;
    var page = option.page || 1;
    var totalPages = parseInt((total / size), 10) + ((total % size) > 0 ? 1 : 0) || 1;

    var currOption = {
        startPage: page,
        totalPages: totalPages,
        visiblePages: 10,
        first: "First",
        prev: "Previous",
        next: "Next",
        last: "Last",
        onPageClick: function (event, page) {
            if (!dsApp.pagers[query]) {
                dsApp.pagers[query] = true;
                return;
            }
            click(page);
        }
    };

    if (dsApp.pagers.hasOwnProperty(query)) {
        $(query).twbsPagination("destroy");
    }
    dsApp.pagers[query] = false;
    $(query).twbsPagination(currOption);
}

function setMenuActive(id) {
    $('li.active').attr('class', '');
    $('#' + id).attr('class', 'active');
}

function logout() {
    dsApp.post('/Account/Logout');
}