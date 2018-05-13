/**
* testFlask.js v1.0.0 by Fatih Sahin
* MIT License
* https://github.com/FatihSahin/test-flask
*/
var testFlask = {};

(function createTestFlask($this) {

    //testFlask.js must be initialized in a razor page script right after testFlask.js is included in document
    $this.initialize = function(options) {
        $this.recordMode = options.recordMode;
        $this.currentScenarioNo = options.scenarioNo;
        $this.overwriteStepNo = options.overwriteStepNo;
        $this.managerScenarioBaseUrl = options.managerScenarioBaseUrl;
        $this.actions = options.actions;

        $this.loadScenarios();
    };

    $this.prepareRequest = function (action) {
        var httpRequest = new XMLHttpRequest();

        httpRequest.onreadystatechange = () => {
            if (httpRequest.readyState === 4 && httpRequest.status === 200) {
                action();
            }
        };

        var loadingGif = document.getElementById("testFlaskRefreshLoadingGif");

        httpRequest.addEventListener("progress", () => {
            loadingGif.style.display = 'inline';
        });

        httpRequest.addEventListener("loadend", () => {
            setTimeout(() => {
                loadingGif.style.display = 'none';
            }, 500);
        });

        return httpRequest;
    }

    $this.toggleView = function () {
        var httpRequest = $this.prepareRequest(() => {
            var elem = document.getElementById("testFlaskAssistantBody");
            elem.style.display = elem.style.display !== 'block' ? 'block' : 'none';
            var refreshLink = document.getElementById("testFlaskRefresh");
            refreshLink.style.display = elem.style.display === 'none' ? 'none' : 'inline';
        });

        httpRequest.open("GET", $this.actions["toggleView"], true);
        httpRequest.send();
    };

    $this.createScenario = function () {
        var elem = document.getElementById("testFlaskCreateNewScenario");
        elem.style.display = 'block';
        var steps = document.getElementById("testFlaskSteps");
        steps.style.display = 'none';
    };

    $this.cancelNewScenario = function () {
        var elem = document.getElementById("testFlaskCreateNewScenario");
        elem.style.display = 'none';
        var steps = document.getElementById("testFlaskSteps");
        steps.style.display = 'block';
        var nameText = document.getElementById("testFlaskNewScenarioName");
        nameText.value = '';
    };

    $this.saveNewScenario = function () {
        var httpRequest = $this.prepareRequest(() => {
            $this.cancelNewScenario(); //hide new scenario view
            $this.loadScenarios();
        });

        var scenarioName = document.getElementById("testFlaskNewScenarioName").value;

        httpRequest.open("POST", $this.actions["createNewScenario"], true);
        httpRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        httpRequest.send(`scenarioName=${scenarioName}`);
    };

    $this.loadScenarios = function () {
        var httpRequest = $this.prepareRequest(() => {
            var elem = document.getElementById("testFlaskScenariosDropdown");

            $this.removeOptions(elem);

            var scenarios = JSON.parse(httpRequest.responseText);
            scenarios.forEach(sce => {
                var option = document.createElement("option");
                option.text = sce.ScenarioName;
                option.value = sce.ScenarioNo;
                option.selected = sce.ScenarioNo.toString() === $this.currentScenarioNo.toString();
                elem.add(option);
            });

            if ($this.currentScenarioNo) {
                $this.loadSteps($this.currentScenarioNo);
            }
            else if (elem.options.length > 0) {
                $this.loadSteps(elem.options[0].value);
            }
        });

        httpRequest.open("GET", $this.actions["getScenarios"], true);
        httpRequest.send();
    };

    $this.removeOptions = function (selectbox) {
        var i;
        for (i = selectbox.options.length - 1; i >= 0; i--) {
            selectbox.remove(i);
        }
    };

    $this.loadSteps = function (scenarioNo) {
        var httpRequest = $this.prepareRequest(() => {
            $this.currentScenarioNo = scenarioNo;
            var elem = document.getElementById("testFlaskSteps");
            elem.innerHTML = httpRequest.responseText;
            var recordCheckox = document.getElementById("testFlaskRecordCheckBox");
            recordCheckox.checked = $this.recordMode;
            $this.autoReload(recordCheckox.checked);
        });

        httpRequest.open("POST", $this.actions["steps"], true);
        httpRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        httpRequest.send(`scenarioNo=${scenarioNo}`);
    };

    $this.autoReload = function (shouldRecord) {
        //auto refresh scenarios when record mode is on and a specific step is not selected for overwriting and not already reload on
        if (shouldRecord && !$this.overwriteStepNo && !$this.reloadIntervalHandle) {
            $this.reloadIntervalHandle = setInterval($this.loadScenarios, 4000);
        }

        if (!shouldRecord && $this.reloadIntervalHandle) {
            clearInterval($this.reloadIntervalHandle); //stop auto refresh when record mode is off
            $this.reloadIntervalHandle = null;
        }
    }

    $this.record = function (scenarioNo) {
        var httpRequest = $this.prepareRequest(() => {
            //do nothing
        });

        var shouldRecord = document.getElementById("testFlaskRecordCheckBox").checked;
        $this.recordMode = shouldRecord;
        $this.autoReload(shouldRecord);

        httpRequest.open("POST", $this.actions["record"], true);
        httpRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        httpRequest.send(`scenarioNo=${scenarioNo}&stepNo=${$this.overwriteStepNo}&record=${shouldRecord}`);
    };

    $this.overwriteStep = function (stepNo) {
        var overwriteLink = document.getElementById("testFlaskStepOverwrite_" + stepNo);
        var recordChk = document.getElementById("testFlaskRecordCheckBox");

        if (!$this.overwriteStepNo) {
            recordChk.checked = true;
            $this.recordMode = true;
            $this.overwriteStepNo = stepNo;
            $this.record($this.currentScenarioNo);

            overwriteLink.innerHTML = "Stop";
        }
        else {
            recordChk.checked = false;
            $this.recordMode = false;
            $this.overwriteStepNo = 0;
            $this.record($this.currentScenarioNo);

            overwriteLink.innerHTML = "Overwrite";
        }
    };

    $this.editStep = function (stepNo) {
        var textBox = document.getElementById("testFlaskStepName_" + stepNo);
        textBox.readOnly = false;

        var editActionsSpan = document.getElementById("testFlaskStepEditActions_" + stepNo);
        editActionsSpan.style.display = 'inline';
        var defaultActionsSpan = document.getElementById("testFlaskStepDefaultActions_" + stepNo);
        defaultActionsSpan.style.display = 'none';
    };

    $this.saveStep = function (stepNo) {

        var httpRequest = $this.prepareRequest(() => {
            $this.cancelStep(stepNo);
            $this.loadScenarios();
        });

        var stepName = document.getElementById("testFlaskStepName_" + stepNo).value;

        httpRequest.open("POST", $this.actions["updateStep"], true);
        httpRequest.setRequestHeader("Content-type", "application/x-www-form-urlencoded");
        httpRequest.send(`stepNo=${stepNo}&stepName=${stepName}`);
    };

    $this.cancelStep = function (stepNo) {
        var textBox = document.getElementById("testFlaskStepName_" + stepNo);
        textBox.readOnly = true;

        var defaultActionsSpan = document.getElementById("testFlaskStepDefaultActions_" + stepNo);
        defaultActionsSpan.style.display = 'inline';
        var editActionsSpan = document.getElementById("testFlaskStepEditActions_" + stepNo);
        editActionsSpan.style.display = 'none';
    };

    $this.manageScenario = function () {
        var elem = document.getElementById("testFlaskScenariosDropdown");
        var scenarioNo = elem.value;
        var scenarioManagerUrl = $this.managerScenarioBaseUrl + '/' + scenarioNo;
        window.open(scenarioManagerUrl);
    };

    $this.refresh = function () {
        $this.loadScenarios();
    };

})(testFlask);