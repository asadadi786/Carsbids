import { create } from "zustand"

type State = {
    pageNumber: number
    pageSize: number
    pageCount: number
    searchTerm: string
    searchValue: string
    orderBy: string
    filterBy: string
}

type Actions = {

    ////method to set one param at a time using Partial<State>,without defining params as null in type State.
    setParams: (params: Partial<State>) => void 
    reset: () => void
    setSearchValue: (value: string) => void
}

const initialState: State = {
    pageNumber: 1,
    pageSize: 12,
    pageCount: 1,
    searchTerm: '',
    searchValue: '',
    orderBy: 'make',
    filterBy: 'live'
}

//create our store
export const useParamsStore = create<State & Actions>()((set) => ({
    ...initialState,

    setParams: (newParams: Partial<State>) => {
        set((state) => {
            if (newParams.pageNumber) {
                return {...state, pageNumber: newParams.pageNumber}
            } else {
                return {...state,...newParams, pageNumber: 1}
            }
        })
    },

    reset: () => set(initialState),

    setSearchValue: (value: string) => {
        set({searchValue: value})
    }
}))