'use client'

import React, { useState } from 'react'
import Image from 'next/image'

type Props = {
    imageUrl : string; 
}

export default function CarImage({imageUrl} : Props) {
    const [isLoading, setLoading] = useState(true); 
    return (
            <Image
                src={imageUrl}
                alt='Image'
                fill
                priority
                className=
                    {`object-cover
                    group-hover:opacity-75
                    duration-700
                    ease-in-out
                    ${isLoading ? 'grayscale blur-2xl scale-110' : 'grayscale-0 blur-0 scale-100'}
                    `}
                sizes='(max-width:786px) 100vw, (max-width: 1200pz) 50vw, 25vw'
                onLoadingComplete={() => setLoading(false)}
            />
  )
}
