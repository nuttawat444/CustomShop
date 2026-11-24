const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");

const read = path => readFileSync(join(__dirname, "..", path), "utf8");

test("the mechanic queue shows assigned work and progress without manual creation or priority", () => {
    const view = read("Views/Mechanic/Index.cshtml");

    assert.doesNotMatch(view, /Mechanic-create|CreateJob|เปิดใบงาน|ความสำคัญ|งานด่วนพิเศษ|job\.Priority/);
    assert.match(view, /ยังไม่มีงานที่ได้รับมอบหมาย/);
    assert.match(view, /asp-action="UpdateProgress"/);
    assert.match(view, /เพิ่ม 25%/);
    assert.match(view, /เสร็จสิ้น/);
});

test("the mechanic app creates work through assignment without reading or writing priority", () => {
    const controller = read("Controllers/MechanicController.cs");
    const service = read("Services/QuoteWorkService.cs");
    const entities = read("Models/ShopEntities.cs");
    const dashboard = read("ViewModels/MechanicDashboardViewModel.cs");

    assert.doesNotMatch(controller, /CreateJob|UpdatePriority|UrgentJobsCount/);
    assert.doesNotMatch(dashboard, /NewJobForm|UrgentJobsCount/);
    assert.doesNotMatch(service, /\bPriority\b/);
    assert.doesNotMatch(entities, /\bPriority\b/);
    assert.match(service, /new MechanicJob/);
    assert.match(service, /new JobAssignee/);
});
