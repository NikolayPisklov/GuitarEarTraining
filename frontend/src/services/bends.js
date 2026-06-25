import { useCsrfStore } from "../plugins/piniaStorage"
import axiosInstance from "./api"

export async function submitWholeStepBend(file){
  const csrfStore = useCsrfStore()

  const fromData = new FormData()
  fromData.append('file', file)

  const response = await axiosInstance.post('bends/whole-step', fromData, {
    headers: {
      "Content-Type": "multipart/form-data",
      'X-CSRF-TOKEN': csrfStore.token
    }
  })
  return response.data
}
export async function submitHalfStepBend(file){
  const csrfStore = useCsrfStore()

  const fromData = new FormData()
  fromData.append('file', file)

  const response = await axiosInstance.post('bends/half-step', fromData, {
    headers: {
      "Content-Type": "multipart/form-data",
      'X-CSRF-TOKEN': csrfStore.token
    }
  })
  return response.data
}
