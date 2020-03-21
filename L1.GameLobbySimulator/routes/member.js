const express = require('express');
const router = express.Router();
const GameClient = require("l1.client.node-js")();

router.post('/member/notify', async (req, res) => {
  try {
    const result = await GameClient.member.notifyLogin(req.body);
    res.send(result);
  } catch (error) {
    res.send({ error })
  }
});

router.post('/member/balance', async (req, res) => {
  try {
    const result = await GameClient.member.balance(req.body);
    res.send(result);
  } catch (error) {
    res.send({ error })
  }
});

router.post('/member/bet', async (req, res) => {
  try {
    const result = await GameClient.member.placeBet(req.body);
    res.send(result);
  } catch (error) {
    res.send({ error })
  }
});

module.exports = router;
