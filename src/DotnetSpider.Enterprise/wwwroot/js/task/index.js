$(function () {
	//setMenuActive('tasks');
	var cron;
	var interval = null;
	var emptyTask = {
		id: 0,
		name: '',
		version: '',
		os: 'All',
		applicationName: 'dotnet',
		isEnabled: true,
		cron: '',
		nodesCount: 1,
		arguments: '',
		owners: '',
		developers: '',
		analysts: '',
		tags: ''
	};
	var tasksVUE = new Vue({
		el: '#taskContent',
		data: {
			tasks: [],
			total: 0,
			size: 10,
			keyword: '',
			page: 1,
			newTask: $.extend({},emptyTask),
			errorText: {
				name: '', applicationName: '', version: '', arguments: '', developers: '', owners: '', analysts: ''
			}
		},
		mounted: function () {
			this.$data.page = 1;
			loadTasks(this);
		},
		computed: {
			buttonState:function(){
				return this.nameVdt && this.validateEmpty && this.versionVdt && this.argumentsVdt && this.applicationNameVdt && this.cronVdt && this.developersVdt && this.ownersVdt;
			},
			nameVdt: function () {
				return this.validateEmpty(this, 'name', true);
			},
			versionVdt: function () {
				return this.validateEmpty(this, 'version', true);
			},
			argumentsVdt: function () {
				var value = this.newTask['arguments'];
				if (this.validateEmpty(this, 'arguments', false)) {
					if (value.indexOf('-s:')>=0 || value.indexOf('-i:')>=0) {
						this.errorText["arguments"] = 'Arguments can not be -s or -i';
						return false;
					}
					delete this.errorText["arguments"];
					return true;
				}
				return false;
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
			},
			developersVdt: function () {
				return this.validateEmpty(this, 'developers', true);
			},
			ownersVdt: function () {
				return this.validateEmpty(this, 'owners', true);
			},
			analystsVdt: function () {
				return this.validateEmpty(this, 'analysts', true);
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
				tasksVUE.$data.page = 1;
				tasksVUE.$data.keyword = $('#queryKeyword').val();
				loadTasks(tasksVUE);
			},
			onKeyPress: function (evt) {
				if (evt.charCode === 13) {
					this["query"]();
				}
			},
			saveSpider: function () {
				if (Object.getOwnPropertyNames(this.errorText).length > 1) return;

				var task = this.newTask;
				if (task.id > 0) {
					dsApp.post("/task/modify", task, function () {
						$("#CreateNewTaskModal").modal("hide");
					});
				}
				else {
					dsApp.post("/task/add", task, function () {
						$("#CreateNewTaskModal").modal("hide");
						loadTasks(tasksVUE);
					});
				}
			},
			run: function (task) {
				if (task.running) return;
				dsApp.post("/task/run", { taskId: task.id }, function () {
					swal("Operation Succeed!", "Task is prepare to run.", "success");
					//setTimeout(loadStatus, 3000);
				});
			},
			deleteTask: function (task) {
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
					dsApp.post("/task/remove", { taskId: task.id }, function () {
						swal("Operation Succeed!", "Task was removed.", "success");
						loadTasks(that);
					});
				});
			},
			addTask:function(){
				var _$modal = $('#CreateNewTaskModal');
				tasksVUE.$data.newTask = $.extend({}, emptyTask);
				_$modal.modal('show');
			},
			modifyTask: function (task) {
				var _$modal = $('#CreateNewTaskModal');
				tasksVUE.$data.newTask = task;
				_$modal.modal('show');
			}
		}
	});

	$("#saveTrigger").bind("click", function () {
		var info = cron.getCron();
		var _$modal = $('#SchedulerModal');
		_$modal.modal('hide');
		tasksVUE.$data.newTask.cron = info.cron;
	});

	function queryString(name) {
		var result = location.search.match(new RegExp("[\?\&]" + name + "=([^\&]+)", "i"));
		if (result === null || result.length < 1) {
			return "";
		}
		return result[1];
	}

	var lastQuery;

	function loadStatus() {
		var tasks = tasksVUE.$data.tasks;
		var ids = [];
		$(tasks).each(function () {
			ids.push(this.id);
		});
	}

	function loadTasks(vue) {
		var url = '/Task/GetList';
		var keywrod = vue.$data.keyword || '';
		var query = {  page: vue.$data.page, size: vue.size, keyword: keywrod };

		dsApp.post(url, query, function (result) {
			var rd = result.result.result;

			$(rd).each(function () {
				this.running = false;
			});

			vue.$data.tasks = rd;
			vue.$data.total = result.result.total;

			if (interval) {
				clearInterval(interval);
			}
			if (result.result.result.length > 0) {
				loadStatus();
			}

			dsApp.ui.initPagination('#pagination', result.result, function (page) {
				vue.$data.page = page;
				loadTasks(vue);
			});
		});
	}

	setTimeout(function () {
		$(".menu li").removeClass("active");
		$(".menu li a").removeClass("toggled");
		$(".menu li#tasks").addClass("active");
		$(".menu li#tasks a").addClass("toggled");
	}, 50);
});