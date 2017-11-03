$(function () {
    var interval = null, statusInterval=null;
    //setMenuActive('tasks');
    var tasksVUE = new Vue({
        el: '#taskContent',
        data: {
            tasks: [],
            nodes: [],
            logs: [],
            identity: '',
            node: '',
            stopable: false,
            pauseable: false,
            continueable:false,
            logsPaging: {
                total: 0,
                size: 10,
                keyword: '',
                page: 1,
            }
        },
        mounted: function () {
            loadTasks(this);
        },
        methods: {
            viewNodes: function (identity) {
                this.identity = identity;
                $("#NodesModal").modal("show");
                if (interval) clearInterval(interval);
                loadStatus();
                if (statusInterval) clearInterval(statusInterval);
                statusInterval = setInterval(loadStatus, 5000);
            },
            clearIntervalStatus: function () {
                if (statusInterval) clearInterval(statusInterval);
                loadTasks(this);
            },
            viewBatchLogs: function () {
                var that = this;
                $("#logType").val('');
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                that.node = '';
                lastQuery = null;
                $("#LogsModal").modal("show");
                loadBatchLogs();
            },
            viewLogs: function (node) {
                var that = this;
                $("#logType").val('');
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                that.node = node;
                lastQuery = null;
                $("#LogsModal").modal("show");
                loadBatchLogs();
            },
            searchResults: function () {
                var that = this;
                that.logsPaging.total = 0;
                that.logsPaging.page = 1;
                lastQuery = null;
                loadBatchLogs();
            },
            stopTask: function (identity) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Terminate task running!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: false
                }, function () {
                    dsApp.post("/Task/StopTask", { identity: that.identity }, function () {
                        swal("Operation Succeed!", "Stop request was applied.", "success");
                    });
                });
            },
            pauseTask: function () {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Pause task running!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: false
                }, function () {
                    dsApp.post("/Task/PauseTask", { identity: that.identity }, function () {
                        swal("Operation Succeed!", "Pause request was applied.", "success");
                    });
                });
            },
            continueTask: function(){
                dsApp.post("/Task/ResumeTask", { identity: this.identity }, function () {
                    swal("Operation Succeed!", "Resume request was applied.", "success");
                });
            }
        }
    });

    var lastQuery;

    function loadBatchLogs() {
        var query;
        if (!lastQuery) {
            query = {
                identity: tasksVUE.$data.identity,
                node: tasksVUE.$data.node,
                startDate: $("#startDate").val(),
                endDate: $("#endDate").val(),
                page: tasksVUE.$data.logsPaging.page,
                size: tasksVUE.$data.logsPaging.size,
                logType: $("#logType").val(),
            };
            lastQuery = query;
        }
        else query = lastQuery;

        dsApp.post('/task/getBatchLogs', query, function (result) {
            tasksVUE.$data.logs = result.result.result;
            tasksVUE.$data.logsPaging.total = result.result.total;

            dsApp.ui.initPagination('#logsPagination', result.result, function (page) {
                tasksVUE.$data.logsPaging.page = page;
                if (lastQuery) lastQuery.page = page;
                loadBatchLogs();
            });
        });
    }

    function loadStatus() {
        dsApp.post('/task/GetNodeStatus', { identity: tasksVUE.$data.identity }, function (result) {
            var nodes = result.result;
            var idx;
            var _did = false;
            for (idx = 0; idx < nodes.length; idx++) {
                if (nodes[idx].status != 'Finished' && nodes[idx].status != 'Exited') {
                    tasksVUE.$data.stopable = true;
                    _did = true;
                    break;
                }
            }
            if (!_did) tasksVUE.$data.stopable = false;
            _did = false;
            for (idx = 0; idx < nodes.length; idx++) {
                if (nodes[idx].status == 'Init' || nodes[idx].status == 'Running') {
                    tasksVUE.$data.pauseable = true;
                    _did = true;
                    break;
                }
            }
            if (!_did) tasksVUE.$data.pauseable = false;
            _did = false;
            for (idx = 0; idx < nodes.length; idx++) {
                if (nodes[idx].status == 'Stopped') {
                    tasksVUE.$data.continueable = true;
                    _did = true;
                    break;
                }
            }
            if (!_did) tasksVUE.$data.continueable = false;
            tasksVUE.$data.nodes = nodes;
            return false;
        });
    }

    function loadTasks(vue) {
        vue = vue || tasksVUE;
        var url = '/Task/GetRunningTasks';

        dsApp.post(url, {}, function (result) {
            vue.$data.tasks = result.result;
            if (interval) clearInterval(interval);
            interval = setInterval(loadTasks, 10000);
        });
    }

    setTimeout(function () {
        $(".menu li").removeClass("active");
        $(".menu li a").removeClass("toggled");
        $(".menu li#taskState").addClass("active");
        $(".menu li#taskState a").addClass("toggled");
    }, 50);
});