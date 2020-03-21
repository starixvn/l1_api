const express = require("express");
const router = express.Router();
const fs = require("fs");

router.get("/setting/data", async (req, res) => {
  const config = await fs.readFileSync("game-client-config.json");
  res.send(config);
});

router.post("/setting/data", async (req, res) => {
  try {
    await fs.writeFileSync(
      "game-client-config.json",
      JSON.stringify(req.body, null, 2)
    );

    res.send(req.body);
    res.send(result);
  } catch (error) {
    res.send({ error });
  }
});

module.exports = router;
