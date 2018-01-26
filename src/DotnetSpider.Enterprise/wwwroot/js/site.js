// Write your Javascript code.
var dsApp = {};

dsApp.queryString = function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result === null || result.length < 1) {
        return "";
    }
    return result[1];
}

dsApp.post = function (url, data, success, error) {
    $.post(url, data, function (result, status, request) {
        if (result && result.status === "Success") {
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
        if (result && result.status === "Success") {
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

dsApp.delete = function (url, success, error) {
    $.ajax({
        url: url,
        type: 'DELETE',
        success: function (result) {
            if (result && result.status === "Success") {
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
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal("Oops...", result.message, "Internal error.");
                }
            }
        }
    });
}


dsApp.put = function (url, data, success, error) {
    $.ajax({
        url: url,
        data: data,
        type: 'PUT',
        success: function (result) {
            if (result && result.status === "Success") {
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
        },
        error: function (result) {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    swal("Oops...", result.message, "Internal error.");
                }
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

dsApp.getFilter = function (key) {
    var filter = dsApp.queryString('filter');
    if (!filter) {
        return '';
    }
    var kvs = filter.split('|');
    var filters = {};
    for (i = 0; i < kvs.length; ++i) {
        var kv = kvs[i].split('::');
        filters[kv[0]] = kv[1];
    }
    return filters[key];
}

function setMenuActive(id) {
    $('li.active').attr('class', '');
    $('#' + id).attr('class', 'active');
}

function logout() {
    dsApp.post('/Account/Logout');
}