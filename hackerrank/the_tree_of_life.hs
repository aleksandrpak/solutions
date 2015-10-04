{- Solution to challenge https://www.hackerrank.com/challenges/the-tree-of-life -}

{-# LANGUAGE ViewPatterns #-}

import Data.Bits
import qualified Data.Map as M

type Rule = (Bool -> Bool -> Bool -> Bool -> Bool)

data Tree = Empty | Node Bool Tree Tree deriving (Show)

parse :: String -> Tree
parse "." = Node False Empty Empty
parse "X" = Node True Empty Empty
parse ['(', l, ' ', v, ' ', r, ')'] = Node (v == 'X') (Node (l == 'X') Empty Empty) (Node (r == 'X') Empty Empty)
parse (id -> '(':l:' ':v:' ':'(':rs) = Node (v == 'X') (Node (l == 'X') Empty Empty) (parse $ '(' : init rs)
parse (reverse -> (')':r:' ':v:' ':')':ls)) = Node (v == 'X') (parse $ drop 1 (reverse ls) ++ ")") (Node (r == 'X') Empty Empty)
parse (_:_:input) = Node (root == 'X') left right
    where
        findRoot :: Int -> Int -> String -> (Int, Char)
        findRoot i count (x1:x2:x3:xs)
            | x1 == '(' = findRoot (i + 1) (count + 1) (x2:x3:xs)
            | x1 == ')' && count > 1 = findRoot (i + 1) (count - 1) (x2:x3:xs)
            | x1 == ')' = (i + 1, x3)
            | otherwise = findRoot (i + 1) count (x2:x3:xs)
        findRoot _ _ _ = error "cannot find root: invalid input"
        (n, root) = findRoot 1 1 input
        left = parse . take n $ '(':input
        right = parse . drop (n + 2) $ init input
parse _ = error "cannot parse tree: invalid input"

rule :: Int -> Rule
rule r x1 x2 x3 x4 = testBit r i
    where
        y1 = if x1 then 1 else 0
        y2 = if x2 then 1 else 0
        y3 = if x3 then 1 else 0
        y4 = if x4 then 1 else 0
        i = (y1 * 8) + (y2 * 4) + (y3 * 2) + y4

goto :: Tree -> String -> Bool
goto Empty _ = False
goto (Node v _ _) [] = v
goto (Node _ l r) (x:xs)
    | x == '>' = goto r xs
    | x == '<' = goto l xs
goto _ _ = error "invalid path"

getV :: Tree -> Bool
getV (Node x _ _) = x
getV Empty = False

next :: Rule -> Bool -> Tree -> Tree
next _ _ Empty = Empty
next r x1 (Node x3 lt rt) = Node x (next r x3 lt) (next r x3 rt)
    where x = r x1 (getV lt) x3 (getV rt)

buildNext :: Int -> Int -> Int -> Rule -> M.Map Int Tree -> M.Map Int Tree
buildNext step i count r trees
    | i + step <= count = trees
    | otherwise = buildNext step i (count + 1) r $ M.insert (count + 1) (next r False (M.findWithDefault Empty count trees)) trees

test :: Int -> Rule -> M.Map Int Tree -> Int -> Int -> IO ()
test 0 _ _ _ _ = return ()
test steps r trees i count = do
    input <- fmap words getLine
    let
        step = read $ head input
        path = init . tail $ last input
        newTrees = buildNext step i count r trees
        key = step + i
        tree = M.findWithDefault Empty key newTrees
        v = if goto tree path then "X" else "."
    putStrLn v
    test (steps - 1) r newTrees key (max key count)

main :: IO ()
main = do
    r <- fmap (rule . read) getLine
    tree <- fmap parse getLine
    steps <- fmap read getLine
    test steps r (M.insert 0 tree M.empty) 0 0
