const express = require('express');
const router = express.Router();
const GameClient = require("l1.client.node-js")();

router.post('/signin/sso', async (req, res) => {
  try {
    const gameId = req.body.gameId;
    const result = await GameClient.auth.singleSignOn(req.body);
    const url = new URL(result.redirectUrl);
    const token = url.searchParams.get("token");
    
    res.send({token, redirectUrl: `${result.redirectUrl}&gameId=${gameId}`});
  } catch (error) {
    res.send({ error })
  }
});

router.post('/signin/verify', async (req, res) => {
  try {
    const result = await GameClient.auth.verifyToken(req.body);

    res.send(result);
  } catch (error) {
    res.send({ error })
  }
});

module.exports = router;
