import axiosInstance from "./api"

export async function submitBendFile(file){
  const fromData = new FormData()
  fromData.append('file', file)

  await axiosInstance.post('bends/whole-step', fromData, {
    headers: {
      "Content-Type": "multipart/form-data"
    }
  })
}