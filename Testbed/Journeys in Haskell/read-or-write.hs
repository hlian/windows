import IO

glasses filename =
	bracket (openFile filename ReadMode) hClose (\h -> do
		contents <- hGetContents h
		putStrLn contents
		return() -- Needed, although not in tutorial
	)

beavers filename = do
	putStrLn "Enter text"
	contents <- getLine
	bracket (openFile filename WriteMode)
		hClose
		(\h -> hPutStrLn h contents)

askance = do
	putStrLn "Read, write, or quit?"
	command <- getLine
	case command of
		'q':_ -> return ()
		'r':filename -> do
				putStrLn ("Reading " ++ filename)
				glasses filename
				askance
		'w':filename -> do
				putStrLn ("Writing " ++ filename)
				beavers filename
				askance
		_:command -> do
			putStrLn ("I don't understand " ++ command)
			askance