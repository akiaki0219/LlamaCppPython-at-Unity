from llama_cpp import Llama

def LlamaCpp(modelName, chatText, tokens):
  llm = Llama(model_path = modelName, chat_format = "llama-3", n_ctx=1024,)

  response = llm.create_chat_completion(messages=[{"role": "user", "content": chatText,},], max_tokens=tokens,)

  return response["choices"][0]["message"]["content"]
