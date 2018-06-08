$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            tasks: [],
            page: hub.queryString('page') || 1,
            size: hub.queryString('size') || 30,
            total: 0,
            keyword: decodeURIComponent(hub.queryString('keyword') || '')
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            query: function () {
                window.location.href = 'task?keyword=' + $('#queryKeyword').val() + '&page=' + 1 + '&size=' + this.$data.size;
            },
            onKeyPress: function (evt) {
                if (evt && evt.charCode === 13) {
                    view.$data.page = 1;
                    this.query();
                }
            },
            run: function (task) {
                var that = this;
                hub.get("api/v1.0/task/" + task.id + '?action=run', function () {
                    swal("Operation succeed.", "", "success");
                    loadView(that);
                });
            },
            remove: function (task) {
                var that = this;
                swal({
                    title: "Sure to remove this task?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.delete("api/v1.0/task/" + task.id, function () {
                        loadView(that);
                    });
                });
            },
            exit: function (id) {
                var that = this;
                swal({
                    title: "Sure to exit this task?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.get("api/v1.0/task/" + id + '?action=exit', function () {
                        swal("Operation succeed.", "", "success");
                        loadView(that);
                    });
                });
            },
            disable: function (task) {
                var that = this;
                swal({
                    title: "Sure to disable this task?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.get("api/v1.0/task/" + task.id + '?action=disable', function () {
                        swal("Operation succeed!", "", "success");
                        loadView(that);
                    });
                });
            },
            enable: function (task) {
                var that = this;
                hub.get("api/v1.0/task/" + task.id + '?action=enable', function () {
                    swal("Operation succeed!", "", "success");
                    loadView(that);
                });
            }
        }
    });

    function loadView(vue) {
        var url = 'api/v1.0/task?keyword=' + vue.$data.keyword + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        hub.get(url, function (result) {
            vue.$data.tasks = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;
            hub.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'task?keyword=' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});