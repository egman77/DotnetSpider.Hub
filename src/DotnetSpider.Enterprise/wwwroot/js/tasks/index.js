$(function () {
	//setMenuActive('tasks');
	function appendError(id, target, error) {
		var el = $(target).parent();
		el.addClass("error");
		if ($("#" + id + "-error").length) {
			$("#" + id + "-error").text(error);
		}
		else {
			el.parent().append('<label id="' + id + '-error" class="error" for="email">' + error + '</label>');
		}
	}
	function removeError(id, target) {
		$(target).parent().removeClass("error");
		$("#" + id + "-error").remove();
	}
	var validator = {
		assemblyName:  function (target,value) {
			if ($.trim(value) == '') {
				appendError('assemblyName', target, 'Assembly name can not be empty.');
				return false;
			}
			removeError('assemblyName', target);
			return true;
		}
	};

	var proj = queryString("proj");
	var interval = null;
	var tasksVUE = new Vue({
		el: '#taskContent',
		data: {
			tasks: [],
			total: 0,
			size: 10,
			keyword: '',
			page: 1,
			version: {
				page: 1,
				size: 10,
				total: 0
			},
			historyVersion: {
				page: 1,
				size: 10,
				total: 0
			},
			newTask: {
				id: 0,
				name:'',
				version: '',
				framework:'NetCore',
				assemblyName:'',
				taskName: '',
				projectId: 0,
				isEnabled: true,
				cron: '',
				nodesCount: 1,
				extraArguments: ''
			},
			//projVersion: [],
			//templateVersion: {},
			//versions: [],
			//currentVersion: '',
			taskId: 0
		},
		mounted: function () {
			this.$data.page = 1;
			this.$data.newTask.projectId = proj;
			loadTasks(this);
		},
		computed: {
			//selSpiderName: {
			//	get: function () {
			//		if (this.newTask.spiderName == '') return '';
			//		return this.newTask.spiderName + '-[' + this.newTask.version + ']';
			//	}
			//}
		},
		methods: {
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
				if (evt.charCode == 13) {
					this["query"]();
				}
			},
			//backdateVersion: function (task) {
				//if (this.taskId != task.id || this.versions.length == 0) {
				//	this.currentVersion = task.version;
				//	this.taskId = task.id;
				//	this.versions = [];
				//	loadVersions(this);
				//}
				//$("#VersionModal").modal("show");
			//},
			//setVersion: function (version) {
			//	var that = this;
			//	swal({
			//		title: "Are you sure?",
			//		text: "Set new version for this task!",
			//		type: "warning",
			//		showCancelButton: true,
			//		confirmButtonColor: "#DD6B55",
			//		confirmButtonText: "Yes, do it!",
			//		closeOnConfirm: false
			//	}, function () {
			//		$("#VersionModal").modal("hide");
			//		dsApp.post("/Task/SetVersion", { taskId: that.taskId, version: version }, function () {
			//			swal("Operation Succeed!", "New Version was applied.", "success");
			//			that.currentVersion = '';
			//			that.taskId = 0;
			//			loadTasks(that);
			//		});
			//	});
			//},
			//onSelectVersion: function () {
			//	loadProjectVersion(this);
			//},
			//searchResults: function () {
			//	lastQuery = null;
			//	loadProjectVersion(this);
			//},
			//selectModalVersion: function (item) {
			//	this.templateVersion = item;
			//},
			//selectSpider: function () {
			//	this.newTask.version = this.templateVersion.version;
			//	this.newTask.spiderName = $("input[name='radiobox']:checked").val() || $("input[name='radiobox']").val();
			//},
			saveSpider: function () {
				var that = this;

				if (this.newTask.name == '') {
					swal('Choose spider first!'); return;
				}
				if (this.newTask.name == '') {
					swal('Name can not be empty!'); return;
				}

				dsApp.post("/task/addTask", this.newTask, function () {
					$("#CreateNewTaskModal").modal("hide");
					loadTasks(that);
				});
			},
			run: function (task) {
				if (task.running) return;
				dsApp.post("/task/runTask", { taskId: task.id }, function () {
					swal("Operation Succeed!", "Task is prepare to run.", "success");
					setTimeout(loadStatus, 3000);
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
					dsApp.post("/task/deleteTask", { taskId: task.id }, function () {
						swal("Operation Succeed!", "Task was removed.", "success");
						loadTasks(that);
					});
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

	//function loadVersions(vue) {
	//	var url = '/Task/GetVersions';
	//	var query = {
	//		taskId: vue.$data.taskId,
	//		page: vue.$data.historyVersion.page,
	//		size: vue.$data.historyVersion.size
	//	}
	//	dsApp.post(url, query, function (result) {
	//		vue.$data.versions = result.result.result;
	//		vue.$data.historyVersion.total = result.result.total;

	//		dsApp.ui.initPagination('#historyVersionPagination', result.result, function (page) {
	//			vue.$data.historyVersion.page = page;
	//			loadVersions(vue);
	//		});
	//	});
	//}

	var lastQuery;

	//function loadProjectVersion(vue) {
	//	var url = '/Project/GetVersions';
	//	var query;
	//	if (!lastQuery) {
	//		query = {
	//			solutionId: proj,
	//			page: vue.$data.version.page,
	//			size: vue.$data.version.size,
	//			keyword: $("#description").val(),
	//			startDate: $("#startDate").val(),
	//			endDate: $("#endDate").val()
	//		};
	//		lastQuery = query;
	//	}
	//	else query = lastQuery;

	//	dsApp.post(url, lastQuery, function (result) {
	//		vue.$data.projVersion = result.result.result;
	//		vue.$data.version.total = result.result.total;

	//		dsApp.ui.initPagination('#versionPage', result.result, function (page) {
	//			vue.$data.version.page = page;
	//			lastQuery.page = page;
	//			loadProjectVersion(vue);
	//		});
	//	});
	//}

	function loadStatus() {
		var tasks = tasksVUE.$data.tasks;
		var ids = [];
		$(tasks).each(function () {
			ids.push(this.id);
		});
		dsApp.post('/Task/IsTaskRunning', { tasks: ids }, function (data) {
			$(tasks).each(function () {
				this.running = $.inArray(this.id, data.result) >= 0;
			});
		});
	}

	function loadTasks(vue) {
		var url = '/Task/GetList';
		var keywrod = vue.$data.keyword || '';
		var query = { solutionId: proj, page: vue.$data.page, size: vue.size, keyword: keywrod };

		dsApp.post(url, query, function (result) {
			var rd = result.result.result;

			$(rd).each(function () {
				this.running = false;
			});

			vue.$data.tasks = rd;
			vue.$data.total = result.result.total;
			vue.$data.solutions = result.projects;

			if (interval) {
				clearInterval(interval);
			}
			if (result.result.result.length > 0) {
				//loadStatus();
				//interval = setInterval(loadStatus, 10000);
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
		$(".menu li#projects").addClass("active");
		$(".menu li#projects a").addClass("toggled");
	}, 50);
});