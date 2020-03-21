const express = require("express");
const router = express.Router();
const GameClient = require("l1.client.node-js")();
const fs = require("fs");

router
  .get("/resource/currency", async (req, res) => {
    const config = await fs.readFileSync("resource/currency.json");
    res.send(JSON.parse(config));
  })
  .post("/resource/currency", async (req, res) => {
    const config = await fs.writeFileSync("resource/currency.json", req.body);
    res.send(config);
  });

router
  .get("/resource/game", async (req, res) => {
    const config = await fs.readFileSync("resource/game.json");
    res.send(JSON.parse(config));
  })
  .post("/resource/game", async (req, res) => {
    const config = await fs.writeFileSync("resource/game.json", req.body);
    res.send(config);
  });

module.exports = router;
