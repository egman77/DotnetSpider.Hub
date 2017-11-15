$(function () {
    setMenuActive("node");
    var nodesVue = new Vue({
        el: '#nodesView',
        data: {
            nodes: [],
            size: 50,
            total: 0,
            sort: '',
            page: 1
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
                    dsApp.post("/Node/Disable", { nodeId: nodeId }, function () {
                        swal("Operation Succeed!", "Node was disabled.", "success");
                        loadNodes(that);
                    });
                });
            },
            enable: function (nodeId) {
                var that = this;
                dsApp.post("/Node/Enable", { nodeId: nodeId }, function () {
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
                    dsApp.post("/Node/Exit", { nodeId: nodeId }, function () {
                        swal("Operation Succeed!", "Message is sent please check it manuly.", "success");
                        loadNodes(that);
                    });
                });
            }
        }
    });


    function loadNodes(vue) {
        var url = '/Node/QueryNodes';
        dsApp.post(url, { page: vue.$data.page, size: vue.size, sort: vue.sort }, function (result) {
            vue.$data.nodes = result.result.result;
            vue.$data.total = result.result.total;
            dsApp.ui.initPagination('#pagination', result.result, function (page) {
                vue.$data.page = page;
                loadNodes(vue);
            });
        });
    }
});

