const express = require("express");
const router = express.Router();
const fs = require("fs");

router
  .get("/resource/:name", async (req, res) => {
    const name = req.params.name;
    const config = await fs.readFileSync(`resource/${name}.json`);
    res.send(JSON.parse(config));
  })
  .post("/resource/:name", async (req, res) => {
    const name = req.params.name;
    await fs.writeFileSync(`resource/${name}.json`, JSON.stringify(req.body, null, 2));
    res.send(req.body);
  });

module.exports = router;
