$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            nodes: [],
            page: hub.queryString('page') || 1,
            size: hub.queryString('size') || 60,
            total: 0
        },
        mounted: function () {
            loadView(this);
        },
        methods: {
            disable: function (nodeId) {
                var that = this;
                swal({
                    title: "Sure to disable this node?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.get("api/v1.0/node/" + nodeId + '?action=disable', function () {
                        loadView(that);
                    });
                });
            },
            enable: function (nodeId) {
                var that = this;
                hub.get("api/v1.0/node/" + nodeId + '?action=enable', function () {
                    loadView(that);
                });
            },
            exit: function (nodeId) {
                var that = this;
                swal({
                    title: "Sure to exit this node?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.get("api/v1.0/node/" + nodeId + '?action=exit', function () {
                        loadView(that);
                    });
                });
            },
            remove: function (nodeId) {
                var that = this;
                swal({
                    title: "Sure to remove this node?",
                    type: "warning",
                    showCancelButton: true
                }, function () {
                    hub.delete("api/v1.0/node/" + nodeId, function () {
                        loadView(that);
                    });
                });
            }
        }
    });

    function loadView(vue) {
        var url = 'api/v1.0/node?page=' + vue.$data.page + '&size=' + vue.$data.size;
        hub.get(url, function (result) {
            vue.$data.nodes = result.data.result;
            vue.$data.total = result.data.total;
            vue.$data.page = result.data.page;
            vue.$data.size = result.data.size;

            hub.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'node?page=' + vue.$data.page + '&size=' + vue.$data.size;
            });
        });
    }
});

