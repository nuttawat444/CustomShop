const assert = require("node:assert/strict");
const { readFileSync } = require("node:fs");
const { join } = require("node:path");
const { test } = require("node:test");

const view = readFileSync(join(__dirname, "../Views/WarehouseWorker/PackingList.cshtml"), "utf8");

test("the packing list distinguishes delivery from pickup and links back to the warehouse", () => {
    assert.match(view, /Model\.Order\.Fulfillment == "delivery"/);
    assert.match(view, /isDelivery \? "จัดส่ง" : "รับที่ร้าน"/);
    assert.match(view, /@if \(isDelivery && !string\.IsNullOrWhiteSpace\(Model\.Order\.Address\)\)[\s\S]*?@Model\.Order\.Address/);
    assert.match(view, /asp-controller="WarehouseWorker" asp-action="Index"/);
});

test("the packing table keeps size and quantity separate and handles empty orders", () => {
    assert.match(view, /Model\.Lines\.Count == 0[\s\S]*?ไม่มีรายการสินค้า/);
    assert.equal((view.match(/<th scope="col"/g) ?? []).length, 3);
    assert.match(view, /line\.Size == StoreSaleRules\.ReadyBikeUnit/);
    assert.match(view, /isReadyBike \|\| string\.IsNullOrWhiteSpace\(line\.Size\) \? "—" : line\.Size/);
    assert.match(view, /@line\.Quantity @\(isReadyBike \? "คัน" : "ชิ้น"\)/);
});
