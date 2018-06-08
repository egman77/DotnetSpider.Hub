var hub = {};

hub.queryString = function (name) {
    var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
    if (result === null || result.length < 1) {
        return "";
    }
    return result[1];
}

hub.post = function (url, data, success, error) {
    $.post(url, data, function (result, status, request) {
        if (result && result.status.toLowerCase() === "success") {
            if (success) {
                success(result);
            }
        } else {
            if (error) {
                error(result);
            } else {
                if (swal) {
                    if (result.message) {
                        swal("Sorry...", result.message, "error");
                    }
                }
            }
        }
    }).error(function (data, status, result) {
        if (error) {
            error(result);
        } else {
            if (swal) {
                swal("Sorry...", result, 'error');
            }
        }
    });
}

hub.get = function (url, success, error) {
    $.get(url, function (result, status, request) {
        if (result && result.status.toLowerCase() === "success") {
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
                        swal("Sorry...", result.message, "error");
                    }
                }
            }
        }
    }).error(function (data, status, result) {
        if (error) {
            error(result);
        } else {
            if (swal) {
                swal("Sorry...", result.message, "error");
            }
        }
    });
}

hub.delete = function (url, success, error) {
    $.ajax({
        url: url,
        type: 'DELETE',
        success: function (result) {
            if (result && result.status.toLowerCase() === "success") {
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
                            swal("Sorry...", result.message, "error");
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
                    swal("Sorry...", result.message, "error");
                }
            }
        }
    });
}


hub.put = function (url, data, success, error) {
    $.ajax({
        url: url,
        data: data,
        type: 'PUT',
        success: function (result) {
            if (result && result.status.toLowerCase() === "success") {
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
                            swal("Sorry...", result.message, "error");
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
                    swal("Sorry...", result.message, "error");
                }
            }
        }
    });
}

hub.ui = {};

hub.ui.setBusy = function () {
    $("#loading").css("display", "");
}
hub.ui.clearBusy = function () {
    $("#loading").css("display", "none");
}

hub.pagers = {};
hub.ui.initPagination = function (query, option, click) {
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
            if (!hub.pagers[query]) {
                hub.pagers[query] = true;
                return;
            }
            click(page);
        }
    };

    if (hub.pagers.hasOwnProperty(query)) {
        $(query).twbsPagination("destroy");
    }
    hub.pagers[query] = false;
    $(query).twbsPagination(currOption);
}

hub.getFilter = function (key) {
    var filter = hub.queryString('filter');
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
