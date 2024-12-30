# PDF Handler API

## Overview
The PDF Handler API provides two endpoints for converting HTML content into PDF files. The API supports two conversion methods: one using a default conversion utility and the other using the Blink rendering engine.

---

## Prerequisites
- .NET Core SDK
- Syncfusion HTML-to-PDF converter library
- `DotNet.RateLimiter` library for rate limiting
- Ensure the application is hosted in an environment where the required dependencies are installed

---

## Endpoints

### 1. Convert HTML to PDF (Default Method)
**URL:** `POST /api/pdf/html-to-pdf`

#### **Description:**
Converts HTML content into a PDF using the default PDF conversion utility.

#### **Request Headers:**
- `Client-Domain` (optional): A custom header for logging the client domain.

#### **Request Body:**
- **HTML Content**: HTML encoded as a byte array.
- **Configuration Parameters:**
  - `PaperSize`: Desired paper size for the PDF.
  - `WithoutPrintStyle`: Whether to exclude print-specific styles.
  - `Layout`: Specifies the layout for the PDF.
  - `WidthPrintPDF`: The width of the printed PDF.
  - `IsFitHeight` (optional): Boolean value to indicate if the content should fit the height of the page.
  - `Top` & `Bottom`: Margins for the top and bottom of the PDF.
  - `FileName` (optional): Name of the generated PDF file.

#### **Response:**
- **Success:**
  - HTTP 200
  - PDF file as a binary stream.
- **Error:**
  - HTTP 500 with error details.

#### **Rate Limit:**
- Maximum 6 requests per second per client.

---

### 2. Convert HTML to PDF (Blink Method)
**URL:** `POST /api/pdf/html-to-pdf-blink`

#### **Description:**
Converts HTML content into a PDF using the Blink rendering engine for more precise styling and layout control.

#### **Request Headers:**
- `Client-Domain` (optional): A custom header for logging the client domain.

#### **Request Body:**
- **HTML Content**: HTML encoded as a byte array.
- **Configuration Parameters:**
  - `MediaType`: Media type for rendering (default: Print).
  - `PdfPageSize`: Desired PDF page size (default: A4).
  - `Scale`: Scaling factor for the PDF (default: 1).
  - `FileName` (optional): Name of the generated PDF file.

#### **Response:**
- **Success:**
  - HTTP 200
  - PDF file as a binary stream.
- **Error:**
  - HTTP 500 with error details.

#### **Rate Limit:**
- Maximum 6 requests per second per client.

---

## Logging
The API logs:
- Total time taken for the conversion
- Source HTML content size
- Client domain (if provided)

---

## Output Files
Converted PDF files are temporarily stored in the `PdfFiles` directory under the application’s root path if using the Blink method.

---

## Error Handling
In case of errors, the API returns detailed logs and a generic error response to the client.

---

## Example Usage

### Request Example (JSON):
```json
POST /api/pdf/html-to-pdf

{
    "HtmlBytes": "<base64-encoded-html>",
    "PaperSize": "A4",
    "WithoutPrintStyle": false,
    "Layout": "portrait",
    "WidthPrintPDF": 800,
    "IsFitHeight": true,
    "Top": 10,
    "Bottom": 10,
    "FileName": "example.pdf"
}
```

### Response Example:
- HTTP 200
- PDF file download with the name `example.pdf`

---

For further questions or issues, please contact the development team.
