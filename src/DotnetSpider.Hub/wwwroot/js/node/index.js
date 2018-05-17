$(function () {
    setMenuActive("node");
    var nodesVue = new Vue({
        el: '#nodesView',
        data: {
            nodes: [],
            page: dsApp.queryString('page') || 1,
            size: dsApp.queryString('size') || 60,
            total: 0
        },
        mounted: function () {
            loadNodes(this);
        },
        methods: {
            disable: function (nodeId) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Disable this node",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: true
                }, function () {
                    dsApp.get("api/v1.0/node/" + nodeId + '?action=disable', function () {
                        swal("Operation Succeed!", "Node was disabled.", "success");
                        loadNodes(that);
                    });
                });
            },
            enable: function (nodeId) {
                var that = this;
                dsApp.get("api/v1.0/node/" + nodeId + '?action=enable', function () {
                    loadNodes(that);
                });
            },
            exit: function (nodeId) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Exit this node",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: true
                }, function () {
                    dsApp.get("api/v1.0/node/" + nodeId + '?action=exit', function () {
                        swal("Operation Succeed!", "Message was sent please check it manuly.", "success");
                        loadNodes(that);
                    });
                });
            },
            remove: function (nodeId) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Remove this node",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: true
                }, function () {
                    dsApp.delete("api/v1.0/node/" + nodeId, function () {
                        swal("Operation Succeed!", "Node was removed.", "success");
                        loadNodes(that);
                    });
                });
            }
        }
    });


    function loadNodes(vue) {
        var url = 'api/v1.0/node?page=' + vue.$data.page + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.nodes = result.data.result;
            vue.$data.total = result.data.total;

            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'node?page=' + vue.$data.page + '&size=' + vue.$data.size;
            });
        });
    }
});

