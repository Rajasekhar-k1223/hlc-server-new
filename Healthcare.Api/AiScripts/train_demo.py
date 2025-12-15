
import sys
import os
import json
import time

# Attempt to import OCR service if available in the same directory
try:
    from ocr_service import extract_text
except ImportError:
    # Quick mock if ocr_service isn't found/configured
    def extract_text(path):
        return {"text": "Mock extracted text for " + path, "success": True}

def train_mock_model(file_path):
    print(f"--- Starting AI Model Training Simulation ---")
    print(f"Input File: {file_path}")
    
    # 1. Data Ingestion (OCR)
    print("\n[Step 1/4] Ingesting Data (OCR Extraction)...")
    time.sleep(1)
    ocr_result = extract_text(file_path)
    
    if not ocr_result.get("success"):
        print(f"Error: Failed to extract text - {ocr_result.get('error')}")
        return
        
    text_data = ocr_result.get("text", "")
    print(f" > Extracted Text: {text_data[:50]}...")
    
    # 2. Preprocessing
    print("\n[Step 2/4] Preprocessing Data (Tokenization, Normalization)...")
    time.sleep(1.5)
    tokens = text_data.split()
    print(f" > Found {len(tokens)} tokens.")
    
    # 3. Model Training (Simulation)
    epochs = 5
    print(f"\n[Step 3/4] Training Model ({epochs} Mock Epochs)...")
    
    current_loss = 0.9
    for epoch in range(1, epochs + 1):
        time.sleep(0.8)
        current_loss *= 0.7  # Simulate loss decreasing
        print(f" > Epoch {epoch}/{epochs}: Loss = {current_loss:.4f}, Accuracy = {(1 - current_loss):.4f}")
        
    # 4. Save Model
    print("\n[Step 4/4] Saving Model Weights...")
    time.sleep(1)
    model_path = "mock_healthcare_model_v1.h5"
    with open(model_path, "w") as f:
        f.write(f"Mock Model trained on {time.ctime()} with data: {text_data[:20]}...")
        
    print(f"\n--- Training Complete! Model saved to {model_path} ---")

    # 5. Send to API
    try:
        import requests
        api_url = "http://localhost:5257/api/ai/training-log"
        payload = {
            "FileName": os.path.basename(file_path),
            "OcrText": text_data[:100], # Trucanting for demo
            "Epochs": epochs,
            "FinalLoss": current_loss,
            "FinalAccuracy": 1 - current_loss,
            "ModelPath": model_path
        }
        response = requests.post(api_url, json=payload)
        if response.status_code == 200:
            print(f" [Success] Training log saved to Database via API. ID: {response.json().get('Id')}")
        else:
            print(f" [Warning] Failed to save log to API. Status: {response.status_code} - {response.text}")
    except ImportError:
        print(" [Error] 'requests' library not found. Skipping API upload.")
    except Exception as e:
        print(f" [Error] Could not connect to API: {e}")

if __name__ == "__main__":
    if len(sys.argv) < 2:
        print("Usage: python train_demo.py <path_to_image_or_pdf>")
        sys.exit(1)
        
    input_file = sys.argv[1]
    train_mock_model(input_file)
