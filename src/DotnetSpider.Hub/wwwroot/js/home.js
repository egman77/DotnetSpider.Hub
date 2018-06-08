$(function () {
    var view = new Vue({
        el: '#view',
        data: {
            taskCount: 0,
            logSize: 0,
            nodeTotalCount: 0,
            nodeOnlineCount: 0,
            runningTaskCount: 0,
            nodes: []
        },
        mounted: function () {
            var url = '/api/v1.0/dashboard';
            var that = this;
            hub.get(url, function (result) {
                that.$data.taskCount = result.data.taskCount;
                that.$data.logSize = result.data.logSize;
                that.$data.nodeTotalCount = result.data.nodeTotalCount;
                that.$data.nodeOnlineCount = result.data.nodeOnlineCount;
                that.$data.runningTaskCount = result.data.runningTaskCount;
                that.$data.nodes = result.data.nodes;
            });
        }
    });
});