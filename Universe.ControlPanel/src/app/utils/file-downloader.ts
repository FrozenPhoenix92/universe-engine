export function downloadJsonFile(data: Blob, fileName: string, mediaType: string): void {
	const downloadLink = document.createElement("a");
	downloadLink.style.display = "none";

	downloadLink.download = fileName;
	downloadLink.href = window.URL.createObjectURL(data);
	downloadLink.dataset.downloadurl = [mediaType, downloadLink.download, downloadLink.href].join(":");

	document.body.appendChild(downloadLink);
	downloadLink.click();
	downloadLink.remove();
}