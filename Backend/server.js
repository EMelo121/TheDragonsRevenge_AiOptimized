import express from "express";
import OpenAI from "openai";

const app = express();
app.use(express.json());

const client = new OpenAI({
  apiKey: process.env.OPENAI_API_KEY
});

app.post("/api/battle-taunt", async (req, res) => {
  try {
    const {
      enemyType,
      enemyStyle,
      enemyLevel,
      areaName,
      isBoss,
      playerLevel,
      battlePhase
    } = req.body;

    const prompt = `
You are writing a very short battle taunt for a fantasy turn-based RPG.

Rules:
- Return ONLY valid JSON.
- Keep the taunt under 16 words.
- Stay in-character for the enemy.
- Use the enemy style if helpful.
- Do not invent mechanics or lore not implied by the data.
- Tone: concise, flavorful, dramatic.

Battle context:
enemyType: ${enemyType}
enemyStyle: ${enemyStyle}
enemyLevel: ${enemyLevel}
areaName: ${areaName}
isBoss: ${isBoss}
playerLevel: ${playerLevel}
battlePhase: ${battlePhase}

Return JSON in this exact format:
{"taunt":"..."}
`;

    const response = await client.responses.create({
      model: "gpt-4.1-mini",
      input: prompt
    });

    const text = response.output_text;

    let parsed;
    try {
      parsed = JSON.parse(text);
    } catch {
      parsed = { taunt: "The enemy glares in silence." };
    }

    res.json(parsed);
  } catch (error) {
    console.error(error);
    res.status(500).json({ taunt: "The enemy says nothing." });
  }
});

app.listen(3000, () => {
  console.log("Battle taunt server running on port 3000");
});