import { useCsrfStore } from "../plugins/piniaStorage"
import axiosInstance from "./api"

export async function submitWholeStepBend(samples, sampleRate){
  const csrfStore = useCsrfStore()
  const response = await axiosInstance.post('bends/whole-step', samples.buffer, {
    headers: {
      "Content-Type": "application/octet-stream",
      "X-Sample-Rate": sampleRate.toString(),
      'X-CSRF-TOKEN': csrfStore.token
    }
  })
  return response.data
}
export async function submitHalfStepBend(samples, sampleRate){
  const csrfStore = useCsrfStore()
  const response = await axiosInstance.post('bends/half-step', samples.buffer, 
    {
      headers: {
        "Content-Type": "application/octet-stream",
        "X-Sample-Rate": sampleRate.toString(),
        'X-CSRF-TOKEN': csrfStore.token
      }
    })
  return response.data
}
