$(function () {
    setMenuActive('task');

    var cron;

    var emptyTask = {
        id: 0,
        name: '',
        version: '',
        os: 'All',
        applicationName: 'dotnet',
        isEnabled: true,
        cron: '',
        nodeCount: 1,
        arguments: '',
        owners: '',
        developers: '',
        analysts: '',
        tags: '',
        nodeType: 1,
        isSingle: true
    };


    var tasksVUE = new Vue({
        el: '#tasksView',
        data: {
            tasks: [],
            page: dsApp.queryString('page') || 1,
            size: dsApp.queryString('size') || 60,
            newTask: $.extend({}, emptyTask),
            total: 0,
            keyword: dsApp.getFilter('keyword') || '',
            isView: false,
            errorText: {
                name: '', applicationName: '', version: '', arguments: ''
            }
        },
        mounted: function () {
            loadTasks(this);
        },
        computed: {
            buttonState: function () {
                return this.nameVdt && this.validateEmpty && this.versionVdt && this.argumentsVdt && this.applicationNameVdt && this.cronVdt & this.nodeCountVdt;
            },
            nameVdt: function () {
                return this.validateEmpty(this, 'name', true);
            },
            nodeCountVdt: function () {
                if (this.validateEmpty(this, 'nodeCount', true)) {
                    var value = this.newTask['nodeCount'];
                    return value >= 1 && value < 100;
                }
                return false;
            },
            versionVdt: function () {
                return this.validateEmpty(this, 'version', true);
            },
            argumentsVdt: function () {
                var value = this.newTask['arguments'];
                if (value && (value.indexOf('-tid:') >= 0 || value.indexOf('-i:') >= 0)) {
                    this.errorText["arguments"] = 'Arguments should not contains -tid or -i';
                    return false;
                }
                delete this.errorText["arguments"];
                return true;
            },
            applicationNameVdt: function () {
                var value = this.newTask['applicationName'];
                if (this.validateEmpty(this, 'applicationName', false)) {
                    if (value.length > 100) {
                        this.errorText["applicationName"] = 'Less than 100 characters.';
                        return false;
                    }
                    delete this.errorText["applicationName"];
                    return true;
                }
                return false;
            },
            cronVdt: function () {
                return this.validateEmpty(this, 'cron', true);
            }
        },
        methods: {
            onTriggerClick: function (event) {
                var _$modal = $('#SchedulerModal');
                cron = $("#cron1").msCron();
                cronId = null;
                _$modal.modal('show');
            },
            validateEmpty: function (vue, v, remove) {
                var value = this.newTask[v];
                if (value === '') {
                    vue.errorText[v] = '';
                    return null;
                }
                else if ($.trim(value) === '') {
                    vue.errorText[v] = 'Value can not be empty.';
                    return false;
                }
                if (remove) {
                    delete vue.errorText[v];
                }
                return true;
            },
            leave: function (evt) {
                if (validator[evt.target.name] && !validator[evt.target.name](evt.target, this.newTask[evt.target.name])) {
                    return;
                }
            },
            query: function () {
                window.location.href = 'task?filter=keyword::' + $('#queryKeyword').val() + '&page=' + 1 + '&size=' + dsApp.queryString('size');
            },
            onKeyPress: function (evt) {
                if (evt.charCode === 13) {
                    tasksVUE.$data.page = 1;
                    this["query"]();
                }
            },
            save: function () {
                if (Object.getOwnPropertyNames(this.errorText).length > 1) return;

                var task = this.newTask;
                if (task.id > 0) {
                    dsApp.put("api/v1.0/task", task, function () {
                        $("#CreateNewTaskModal").modal("hide");
                        loadTasks(tasksVUE);
                    });
                }
                else {
                    dsApp.post("api/v1.0/task", task, function () {
                        $("#CreateNewTaskModal").modal("hide");
                        loadTasks(tasksVUE);
                    });
                }
            },
            run: function (task) {
                var that = this;
                dsApp.get("api/v1.0/task/" + task.id + '?action=run', function () {
                    swal("Operation Succeed!", "Task is prepare to run.", "success");
                    loadTasks(that);
                });
            },
            remove: function (task) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Remove this task!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: false
                }, function () {
                    dsApp.delete("api/v1.0/task/" + task.id, function () {
                        swal("Operation Succeed!", "Task was removed.", "success");
                        loadTasks(that);
                    });
                });
            },
            exit: function (id) {
                var that = this;
                dsApp.get("api/v1.0/task/" + id + '?action=exit', function () {
                    swal("Operation Succeed!", "Task would be exited.", "success");
                    loadTasks(that);
                });
            },
            add: function () {
                var _$modal = $('#CreateNewTaskModal');
                tasksVUE.$data.isView = false;
                tasksVUE.$data.newTask = $.extend({}, emptyTask);
                _$modal.modal('show');
            },
            view: function (task) {
                var _$modal = $('#CreateNewTaskModal');
                tasksVUE.$data.isView = true;
                var item = {};
                for (var prop in task) {
                    if (task.hasOwnProperty(prop)) {
                        item[prop] = task[prop];
                    }
                }
                tasksVUE.$data.newTask = item;
                _$modal.modal('show');
            },
            modify: function (task) {
                var _$modal = $('#CreateNewTaskModal');
                tasksVUE.$data.isView = false;
                var item = {};
                for (var prop in task) {
                    if (task.hasOwnProperty(prop)) {
                        item[prop] = task[prop];
                    }
                }
                tasksVUE.$data.newTask = item;
                _$modal.modal('show');
            },
            disable: function (task) {
                var that = this;
                swal({
                    title: "Are you sure?",
                    text: "Disable this task!",
                    type: "warning",
                    showCancelButton: true,
                    confirmButtonColor: "#DD6B55",
                    confirmButtonText: "Yes, do it!",
                    closeOnConfirm: false
                }, function () {
                    dsApp.get("api/v1.0/task/" + task.id + '?action=disable', function () {
                        swal("Operation Succeed!", "Task was disabled.", "success");
                        loadTasks(that);
                    });
                });
            },
            enable: function (task) {
                var that = this;
                dsApp.get("api/v1.0/task/" + task.id + '?action=enable', function () {
                    swal("Operation Succeed!", "Task was enabled.", "success");
                    loadTasks(that);
                });
            }
        }
    });

    $("#saveTrigger").bind("click", function () {
        var info = cron.getCron();
        var _$modal = $('#SchedulerModal');
        _$modal.modal('hide');
        tasksVUE.$data.newTask.cron = info.cron;
    });

    function loadTasks(vue) {
        var url = 'api/v1.0/task?filter=keyword::' + vue.$data.keyword + '&page=' + vue.$data.page + '&size=' + vue.$data.size;
        dsApp.get(url, function (result) {
            vue.$data.tasks = result.data.result;
            vue.$data.total = result.data.total;
            dsApp.ui.initPagination('#pagination', result.data, function (page) {
                window.location.href = 'task?filter=keyword::' + vue.$data.keyword + '&page=' + page + '&size=' + vue.$data.size;
            });
        });
    }
});