import express from "express";

const app = express();

app.get("/", (req, res) => {
  res.send("Welcome to the jungle.");
});
const port = process.env.PORT || 9000;
app.listen(port, () => {
  console.log(`Listening on port ${port}`);
});
