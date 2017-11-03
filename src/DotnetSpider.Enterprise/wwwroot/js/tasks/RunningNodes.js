$(function () {
    var statusInterval = null;
    var identity = queryString("identity");
    //setMenuActive('tasks');
    var tasksVUE = new Vue({
        el: '#taskContent',
        data: {
            nodes: [],
            identity: '',
            isRunning:false,
            stopable: false,
            pauseable: false,
            continueable: false
        },
        mounted: function () {
            var that = this;
            that.identity = identity;
            dsApp.post("/Task/TaskRunning", { identity: identity }, function (result) {
                that.isRunning = result.result;
                loadStatus();
                if (result.result) {
                    statusInterval = setInterval(loadStatus, 5000);
                }
            });
            
        },
        methods: {
            stopTask: function () {
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
                    dsApp.post("/Task/StopTask", { identity: identity }, function () {
                        swal("Operation Succeed!", "Stop request was applied.", "success");
                        if (statusInterval) statusInterval(statusInterval);
                        that.isRunning = false;
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
                    dsApp.post("/Task/PauseTask", { identity: identity }, function () {
                        swal("Operation Succeed!", "Pause request was applied.", "success");
                    });
                });
            },
            continueTask: function () {
                dsApp.post("/Task/ResumeTask", { identity: identity }, function () {
                    swal("Operation Succeed!", "Resume request was applied.", "success");
                });
            }
        }
    });

    function queryString(name) {
        var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
        if (result == null || result.length < 1) {
            return "";
        }
        return result[1];
    }

    function loadStatus() {
        dsApp.post('/task/GetNodeStatus', { identity: identity }, function (result) {
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

            if (!tasksVUE.$data.stopable && statusInterval) {
                clearInterval(statusInterval);
                statusInterval = null;
            }
            if (!_did) tasksVUE.$data.continueable = false;
            tasksVUE.$data.nodes = nodes;
            return false;
        });
    }

    setTimeout(function () {
        $(".menu li").removeClass("active");
        $(".menu li a").removeClass("toggled");
        $(".menu li#taskState").addClass("active");
        $(".menu li#taskState a").addClass("toggled");
    }, 50);
});