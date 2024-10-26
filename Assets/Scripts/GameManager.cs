using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityPython;

public class GameManager : MonoBehaviour {
	private TextMeshProUGUI modelText; private TextMeshProUGUI inputText; private TextMeshProUGUI sendText; private TextMeshProUGUI replyText; private TextMeshProUGUI tokenText;
	[SerializeField] private LlamaCppConnection llamaCppConnection;
	private string[] modelPath; private int modelIndex; private int modelNum; private string modelName; private string maxTokenText; private int maxToken;

	void OnEnable() {
		modelText = GameObject.Find("ModelText").GetComponent<TextMeshProUGUI>();
		inputText = GameObject.Find("InputText").GetComponent<TextMeshProUGUI>();
		sendText = GameObject.Find("SendText").GetComponent<TextMeshProUGUI>();
		replyText = GameObject.Find("ReplyText").GetComponent<TextMeshProUGUI>();
		tokenText = GameObject.Find("TokenText").GetComponent<TextMeshProUGUI>();
		modelPath = Directory.GetFiles(Application.streamingAssetsPath + "/GGUF/", "*.gguf", SearchOption.TopDirectoryOnly);
		modelIndex = -1; modelNum = modelPath.Count();
	}
	void Start() {
		ChangeModelText();
	}
	void Update() {
		if (Input.GetKeyDown(KeyCode.Return)) {
			SendText();
		}
	}
	
	public void ChangeModelText() {
		modelIndex = (modelIndex+1)%modelNum;
		modelName = Path.GetFileName(modelPath[modelIndex]);
		modelText.text = Path.GetFileNameWithoutExtension(modelName);
	}
	public async void SendText() {
		string userInput = inputText.text;
		Debug.Log(userInput);
		sendText.text = userInput; inputText.text = "";
		replyText.text = "回答生成中";
		maxTokenText = tokenText.text;
		if (maxTokenText.CompareTo("\n") == 1) {
			maxToken = 64;
		}
		else {
			maxToken = int.Parse(maxTokenText.Substring(0, maxTokenText.Length-1));
		}
		string resultText = await llamaCppConnection.LlamaReply(Application.streamingAssetsPath+"/GGUF/"+modelName, userInput, maxToken);
		Debug.Log(resultText);
		replyText.text = resultText;
	}
}
